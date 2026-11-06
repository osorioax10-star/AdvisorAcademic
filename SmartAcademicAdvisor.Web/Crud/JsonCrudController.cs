using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace SmartAcademicAdvisor.Web.Crud;

/// <summary>CRUD completo (listar, crear, editar, eliminar) para cualquier archivo JSON descrito por un CrudSchema.</summary>
[SmartAcademicAdvisor.Web.Perfiles.SoloAdmin]
public abstract class JsonCrudController : Controller
{
    private const string ViewIndex = "~/Views/Shared/Crud/Index.cshtml";
    private const string ViewForm = "~/Views/Shared/Crud/Form.cshtml";
    private const string ViewDelete = "~/Views/Shared/Crud/Delete.cshtml";

    protected readonly JsonFileStore Store;
    protected abstract CrudSchema Schema { get; }

    protected JsonCrudController(IWebHostEnvironment env) => Store = new JsonFileStore(env.ContentRootPath);

    // Ganchos opcionales para cada entidad
    protected virtual void Transform(JsonObject obj, bool isNew) { }
    protected virtual void Validate(JsonObject obj, Dictionary<string, string> errors) { }

    private string NameOf(JsonObject o)
    {
        var n = JsonFileStore.Str(o, Schema.NameKey);
        return n != "" ? n : JsonFileStore.Str(o, "id");
    }

    private Dictionary<string, List<Opt>> LoadOptions()
    {
        var result = new Dictionary<string, List<Opt>>();
        foreach (var f in Schema.Fields.Where(f => f.Kind == FieldKind.Select || f.Kind == FieldKind.Multi))
        {
            var list = new List<Opt>(f.Options);
            if (f.OptionsFile != null)
            {
                foreach (var o in Store.Load(f.OptionsFile).OfType<JsonObject>())
                    list.Add(new Opt(JsonFileStore.Str(o, f.OptionsValue), JsonFileStore.Str(o, f.OptionsLabel)));
            }
            result[f.Key] = list;
        }
        return result;
    }

    private CrudFormVm BuildForm(JsonObject? src, bool isEdit, Dictionary<string, string>? errors = null, IFormCollection? form = null)
    {
        var vm = new CrudFormVm
        {
            Schema = Schema,
            IsEdit = isEdit,
            Options = LoadOptions(),
            Errors = errors ?? new Dictionary<string, string>()
        };
        vm.Id = src != null ? JsonFileStore.Str(src, "id") : (form != null ? form["id"].ToString() : "");
        if (!isEdit && form != null) vm.Values["id"] = form["id"].ToString();

        foreach (var f in Schema.Fields)
        {
            if (form != null)
            {
                vm.Values[f.Key] = form[f.Key].ToString();
                if (f.Kind == FieldKind.Multi)
                {
                    var set = new HashSet<string>();
                    foreach (var s in form[f.Key]) if (!string.IsNullOrEmpty(s)) set.Add(s);
                    vm.Selected[f.Key] = set;
                }
            }
            else if (src != null && src[f.Key] is JsonNode n)
            {
                switch (f.Kind)
                {
                    case FieldKind.Json: vm.Values[f.Key] = n.ToJsonString(JsonFileStore.Pretty); break;
                    case FieldKind.Lines:
                        vm.Values[f.Key] = string.Join("\n", (n as JsonArray)?.Select(x => x?.ToString() ?? "") ?? Enumerable.Empty<string>());
                        break;
                    case FieldKind.Multi:
                        vm.Selected[f.Key] = new HashSet<string>((n as JsonArray)?.Select(x => x?.ToString() ?? "") ?? Enumerable.Empty<string>());
                        break;
                    case FieldKind.Bool: vm.Values[f.Key] = n.ToString().ToLowerInvariant(); break;
                    default: vm.Values[f.Key] = n.ToString(); break;
                }
            }
            else
            {
                vm.Values[f.Key] = f.Default ?? "";
            }
            if (f.Kind == FieldKind.Multi && !vm.Selected.ContainsKey(f.Key)) vm.Selected[f.Key] = new HashSet<string>();
        }
        return vm;
    }

    private void Apply(JsonObject obj, IFormCollection form, Dictionary<string, string> errors)
    {
        foreach (var f in Schema.Fields)
        {
            var raw = form[f.Key].ToString().Trim();
            switch (f.Kind)
            {
                case FieldKind.Bool:
                    obj[f.Key] = raw.Contains("true");
                    break;

                case FieldKind.Multi:
                    var multi = new JsonArray();
                    foreach (var s in form[f.Key]) if (!string.IsNullOrEmpty(s)) multi.Add(s);
                    obj[f.Key] = multi;
                    break;

                case FieldKind.Lines:
                    var lines = new JsonArray();
                    foreach (var l in raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) lines.Add(l);
                    obj[f.Key] = lines;
                    break;

                case FieldKind.Number:
                    if (raw == "")
                    {
                        if (f.Required) errors[f.Key] = "Este campo es obligatorio";
                        else obj[f.Key] = null;
                        break;
                    }
                    if (!double.TryParse(raw.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var num))
                    {
                        errors[f.Key] = "Escribe un número válido";
                        break;
                    }
                    if (f.Min is double mn && num < mn) { errors[f.Key] = $"El mínimo es {mn}"; break; }
                    if (f.Max is double mx && num > mx) { errors[f.Key] = $"El máximo es {mx}"; break; }
                    if (num % 1 == 0 && Math.Abs(num) < 1e15) obj[f.Key] = (long)num;
                    else obj[f.Key] = num;
                    break;

                case FieldKind.Json:
                    if (raw == "")
                    {
                        if (f.Required) errors[f.Key] = "Este campo es obligatorio";
                        else obj[f.Key] = null;
                        break;
                    }
                    try { obj[f.Key] = JsonNode.Parse(raw); }
                    catch (Exception) { errors[f.Key] = "El JSON no es válido (revisa comas, comillas y llaves)"; }
                    break;

                default:
                    if (raw == "" && f.Required) { errors[f.Key] = "Este campo es obligatorio"; break; }
                    if (f.AsNumber && long.TryParse(raw, out var entero)) obj[f.Key] = entero;
                    else obj[f.Key] = raw;
                    break;
            }
        }
    }

    // ---------- Listar ----------
    public virtual IActionResult Index() =>
        View(ViewIndex, new CrudIndexVm
        {
            Schema = Schema,
            Rows = Store.Load(Schema.File).OfType<JsonObject>().ToList(),
            Options = LoadOptions()
        });

    // ---------- Crear ----------
    public virtual IActionResult Create() => View(ViewForm, BuildForm(null, false));

    [HttpPost, ValidateAntiForgeryToken]
    public virtual IActionResult Create(IFormCollection form)
    {
        var errors = new Dictionary<string, string>();
        string id;
        if (Schema.ManualId)
        {
            id = form["id"].ToString().Trim();
            if (id == "") errors["id"] = "El ID es obligatorio";
            else if (Store.Find(Schema.File, id) != null) errors["id"] = "Ya existe un registro con ese ID";
        }
        else id = Store.NextId(Schema.File, Schema.Prefix);

        var obj = new JsonObject { ["id"] = id };
        Apply(obj, form, errors);
        Transform(obj, true);
        Validate(obj, errors);

        if (errors.Count > 0) return View(ViewForm, BuildForm(null, false, errors, form));

        Store.Upsert(Schema.File, obj);
        TempData["Success"] = $"Se creó «{NameOf(obj)}».";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Editar ----------
    public virtual IActionResult Edit(string id)
    {
        var existing = Store.Find(Schema.File, id);
        if (existing == null) return NotFound();
        return View(ViewForm, BuildForm(existing, true));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public virtual IActionResult Edit(string id, IFormCollection form)
    {
        var existing = Store.Find(Schema.File, id);
        if (existing == null) return NotFound();

        var errors = new Dictionary<string, string>();
        var obj = (JsonObject)JsonNode.Parse(existing.ToJsonString())!;   // conserva campos que el formulario no toca
        Apply(obj, form, errors);
        Transform(obj, false);
        Validate(obj, errors);

        if (errors.Count > 0) return View(ViewForm, BuildForm(existing, true, errors, form));

        Store.Upsert(Schema.File, obj);
        TempData["Success"] = $"Se actualizó «{NameOf(obj)}».";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Eliminar ----------
    public virtual IActionResult Delete(string id)
    {
        var existing = Store.Find(Schema.File, id);
        if (existing == null) return NotFound();
        return View(ViewDelete, new CrudDeleteVm { Schema = Schema, Id = id, Name = NameOf(existing) });
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public virtual IActionResult DeleteConfirmed(string id)
    {
        var existing = Store.Find(Schema.File, id);
        var nombre = existing != null ? NameOf(existing) : id;
        var eliminado = Store.Delete(Schema.File, id);
        TempData[eliminado ? "Success" : "Error"] = eliminado ? $"Se eliminó «{nombre}»." : "No se encontró el registro.";
        return RedirectToAction(nameof(Index));
    }
}
