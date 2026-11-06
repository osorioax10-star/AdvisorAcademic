using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using SmartAcademicAdvisor.Core.Models;
using SmartAcademicAdvisor.Services;
using SmartAcademicAdvisor.Web.Crud;
using SmartAcademicAdvisor.Web.Models;

namespace SmartAcademicAdvisor.Web.Controllers;

[SmartAcademicAdvisor.Web.Perfiles.SoloAdmin]
public class EspecialidadesController : Controller
{
    private const string Archivo = "especialidades.json";
    private readonly EspecialidadService _especialidadService;
    private readonly JsonFileStore _store;

    public EspecialidadesController(EspecialidadService especialidadService, IWebHostEnvironment env)
    {
        _especialidadService = especialidadService;
        _store = new JsonFileStore(env.ContentRootPath);
    }

    private void CargarMaterias() =>
        ViewBag.MateriasOpciones = _store.Load("materias.json").OfType<JsonObject>()
            .Select(o => new Opt(JsonFileStore.Str(o, "id"), JsonFileStore.Str(o, "nombre")))
            .OrderBy(o => o.Label).ToList();

    private static string[] Lista(JsonNode? n) =>
        (n as JsonArray)?.Select(x => x?.ToString() ?? "").ToArray() ?? Array.Empty<string>();

    private static JsonArray ComoArreglo(IEnumerable<string>? valores)
    {
        var a = new JsonArray();
        if (valores != null) foreach (var v in valores) a.Add(v);
        return a;
    }

    public async Task<IActionResult> Index()
    {
        var especialidades = await _especialidadService.ObtenerTodasAsync();
        return View(especialidades.OrderBy(e => e.Nombre));
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var especialidad = await _especialidadService.ObtenerPorIdAsync(id);
        if (especialidad == null) return NotFound();
        return View(especialidad);
    }

    // ---------- Crear ----------
    public IActionResult Create()
    {
        CargarMaterias();
        return View(new EspecialidadFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EspecialidadFormModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var especialidad = new Especialidad
                {
                    Id = _store.NextId(Archivo, "ESP"),
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    PromedioMinimo = model.PromedioMinimo,
                    InteresesClave = model.InteresesSeleccionados?.ToList() ?? new List<string>(),
                    MateriasClave = model.MateriasClaveIds?.ToList() ?? new List<string>(),
                    Pesos = new PesosAfinidad
                    {
                        PesoPromedio = model.PesoPromedio,
                        PesoIntereses = model.PesoIntereses,
                        PesoMateriasClave = model.PesoMateriasClave,
                        PesoReglas = model.PesoReglas
                    },
                    CampoLaboral = model.CampoLaboral,
                    CompetenciasRequeridas = model.CompetenciasRequeridas?.Split(',').Select(c => c.Trim()).Where(c => c != "").ToList()
                };

                await _especialidadService.CrearAsync(especialidad);
                TempData["Success"] = "Especialidad creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        CargarMaterias();
        return View(model);
    }

    // ---------- Editar ----------
    public IActionResult Edit(string id)
    {
        var n = _store.Find(Archivo, id);
        if (n == null) return NotFound();

        var pesos = n["pesos"] as JsonObject;
        double D(JsonNode? x, double def) =>
            x != null && double.TryParse(x.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : def;

        CargarMaterias();
        return View(new EspecialidadFormModel
        {
            Id = id,
            Nombre = JsonFileStore.Str(n, "nombre"),
            Descripcion = JsonFileStore.Str(n, "descripcion"),
            PromedioMinimo = D(n["promedioMinimo"], 0),
            InteresesSeleccionados = Lista(n["interesesClave"]),
            MateriasClaveIds = Lista(n["materiasClave"]),
            PesoPromedio = D(pesos?["pesoPromedio"], 0.25),
            PesoIntereses = D(pesos?["pesoIntereses"], 0.30),
            PesoMateriasClave = D(pesos?["pesoMateriasClave"], 0.25),
            PesoReglas = D(pesos?["pesoReglas"], 0.20),
            CampoLaboral = JsonFileStore.Str(n, "campoLaboral"),
            CompetenciasRequeridas = string.Join(", ", Lista(n["competenciasRequeridas"]))
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, EspecialidadFormModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) { CargarMaterias(); return View(model); }

        var todas = _store.Load(Archivo);
        var nodo = todas.OfType<JsonObject>().FirstOrDefault(o => JsonFileStore.Str(o, "id") == id);
        if (nodo == null) return NotFound();

        nodo["nombre"] = model.Nombre;
        nodo["descripcion"] = model.Descripcion;
        nodo["promedioMinimo"] = model.PromedioMinimo;
        nodo["interesesClave"] = ComoArreglo(model.InteresesSeleccionados);
        nodo["materiasClave"] = ComoArreglo(model.MateriasClaveIds);
        nodo["pesos"] = new JsonObject
        {
            ["pesoPromedio"] = model.PesoPromedio,
            ["pesoIntereses"] = model.PesoIntereses,
            ["pesoMateriasClave"] = model.PesoMateriasClave,
            ["pesoReglas"] = model.PesoReglas
        };
        nodo["campoLaboral"] = model.CampoLaboral;
        nodo["competenciasRequeridas"] = ComoArreglo(
            model.CompetenciasRequeridas?.Split(',').Select(c => c.Trim()).Where(c => c != ""));
        _store.Save(Archivo, todas);

        TempData["Success"] = "Especialidad actualizada exitosamente";
        return RedirectToAction(nameof(Index));
    }

    // ---------- Eliminar ----------
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();
        var especialidad = await _especialidadService.ObtenerPorIdAsync(id);
        if (especialidad == null) return NotFound();
        return View(especialidad);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            await _especialidadService.EliminarAsync(id);
            TempData["Success"] = "Especialidad eliminada exitosamente";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
