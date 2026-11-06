using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using SmartAcademicAdvisor.Web.Crud;

namespace SmartAcademicAdvisor.Web.Controllers;

public class ReglasController : JsonCrudController
{
    public ReglasController(IWebHostEnvironment env) : base(env) { }
    protected override CrudSchema Schema => CrudSchemas.Reglas;

    public override IActionResult Index()
    {
        ViewBag.Especialidades = Store.Load("especialidades.json").OfType<JsonObject>()
            .GroupBy(o => JsonFileStore.Str(o, "id"))
            .ToDictionary(g => g.Key, g => JsonFileStore.Str(g.First(), "nombre"));

        return View("Index", new CrudIndexVm
        {
            Schema = Schema,
            Rows = Store.Load(Schema.File).OfType<JsonObject>().ToList()
        });
    }

    protected override void Validate(JsonObject obj, Dictionary<string, string> errors)
    {
        if (!errors.ContainsKey("cabeza") && !(obj["cabeza"] is JsonObject c && c["nombre"] != null && c["argumentos"] is JsonArray))
            errors["cabeza"] = "La cabeza debe ser un objeto con «nombre» y «argumentos» (lista).";

        if (!errors.ContainsKey("cuerpo"))
        {
            if (obj["cuerpo"] is not JsonArray cuerpo || cuerpo.Count == 0)
                errors["cuerpo"] = "El cuerpo debe ser una lista con al menos una premisa.";
            else if (cuerpo.OfType<JsonObject>().Any(p => p["predicado"] == null || p["argumentos"] is not JsonArray))
                errors["cuerpo"] = "Cada premisa necesita «predicado» y «argumentos» (lista).";
        }
    }
}
