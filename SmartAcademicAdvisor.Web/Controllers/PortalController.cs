using Microsoft.AspNetCore.Mvc;
using SmartAcademicAdvisor.Web.Crud;
using SmartAcademicAdvisor.Web.Perfiles;

namespace SmartAcademicAdvisor.Web.Controllers;

/// <summary>Portal del alumno: su panel, su asesor, su plan de estudios y su historial.</summary>
[SoloAlumno]
public class PortalController : Controller
{
    private readonly JsonFileStore _store;

    public PortalController(IWebHostEnvironment env) => _store = new JsonFileStore(env.ContentRootPath);

    private IActionResult Vista(string nombre)
    {
        var (_, alumnoId) = PerfilActual.Leer(Request);
        var datos = alumnoId == null ? null : PortalService.Construir(_store, alumnoId);
        if (datos == null) return RedirectToAction("Salir", "Acceso");
        ViewBag.Alumno = datos;
        return View(nombre, datos);
    }

    public IActionResult Index() => Vista("Index");
    public IActionResult Asesor() => Vista("Asesor");
    public IActionResult Plan() => Vista("Plan");
    public IActionResult Historial() => Vista("Historial");
}
