using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using SmartAcademicAdvisor.Web.Crud;
using SmartAcademicAdvisor.Web.Perfiles;

namespace SmartAcademicAdvisor.Web.Controllers;

/// <summary>Pantalla para elegir con qué perfil entrar: administrador o alumno.</summary>
public class AccesoController : Controller
{
    private readonly JsonFileStore _store;

    public AccesoController(IWebHostEnvironment env) => _store = new JsonFileStore(env.ContentRootPath);

    public IActionResult Index()
    {
        var alumnos = _store.Load("alumnos.json").OfType<JsonObject>()
            .Select(a => new AlumnoResumen(
                JsonFileStore.Str(a, "id"), JsonFileStore.Str(a, "nombre"),
                int.TryParse(JsonFileStore.Str(a, "semestre"), out var s) ? s : 0,
                double.TryParse(JsonFileStore.Str(a, "promedio"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var p) ? p : 0))
            .OrderBy(a => a.Nombre).ToList();
        return View(alumnos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Entrar(string tipo, string? alumnoId)
    {
        if (tipo == "admin")
        {
            PerfilActual.Guardar(Response, "admin");
            return RedirectToAction("Index", "Home");
        }

        if (tipo == "alumno" && !string.IsNullOrWhiteSpace(alumnoId) && _store.Find("alumnos.json", alumnoId) != null)
        {
            PerfilActual.Guardar(Response, "alumno:" + alumnoId);
            return RedirectToAction("Index", "Portal");
        }

        TempData["Error"] = "Elige un perfil válido para continuar.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Salir()
    {
        PerfilActual.Borrar(Response);
        return RedirectToAction(nameof(Index));
    }
}
