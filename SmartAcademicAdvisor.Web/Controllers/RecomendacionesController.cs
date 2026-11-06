using Microsoft.AspNetCore.Mvc;
using SmartAcademicAdvisor.Core.Interfaces;
using SmartAcademicAdvisor.Core.Models;
using SmartAcademicAdvisor.Services;

namespace SmartAcademicAdvisor.Web.Controllers;

[SmartAcademicAdvisor.Web.Perfiles.SoloAdmin]
public class RecomendacionesController : Controller
{
    private readonly IRepository<Recomendacion> _recomendacionRepository;
    private readonly RecommendationService _recommendationService;
    private readonly AlumnoService _alumnoService;
    private readonly MateriaService _materiaService;
    private readonly CertificacionService _certificacionService;
    private readonly ILogger<RecomendacionesController> _logger;

    public RecomendacionesController(
        IRepository<Recomendacion> recomendacionRepository,
        RecommendationService recommendationService,
        AlumnoService alumnoService,
        MateriaService materiaService,
        CertificacionService certificacionService,
        ILogger<RecomendacionesController> logger)
    {
        _recomendacionRepository = recomendacionRepository;
        _recommendationService = recommendationService;
        _alumnoService = alumnoService;
        _materiaService = materiaService;
        _certificacionService = certificacionService;
        _logger = logger;
    }

    /// <summary>Diccionarios id -> nombre para mostrar nombres en lugar de códigos.</summary>
    private async Task CargarCatalogosAsync()
    {
        try
        {
            var alumnos = await _alumnoService.ObtenerTodosAsync();
            ViewBag.Alumnos = alumnos.GroupBy(a => a.Id).ToDictionary(g => g.Key, g => g.First().Nombre);

            var materias = await _materiaService.ObtenerTodasAsync();
            ViewBag.Materias = materias.GroupBy(m => m.Id).ToDictionary(g => g.Key, g => g.First().Nombre);

            // Reflexión: no dependemos de la forma exacta del modelo Certificacion
            var certs = await _certificacionService.ObtenerTodasAsync();
            var mapa = new Dictionary<string, string>();
            foreach (var c in certs)
            {
                var id = c!.GetType().GetProperty("Id")?.GetValue(c)?.ToString();
                var nombre = c.GetType().GetProperty("Nombre")?.GetValue(c)?.ToString();
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(nombre)) mapa[id] = nombre;
            }
            ViewBag.Certificaciones = mapa;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron cargar los catálogos de nombres; se mostrarán los códigos");
        }
    }

    // GET: Recomendaciones
    public async Task<IActionResult> Index()
    {
        try
        {
            await CargarCatalogosAsync();
            var recomendaciones = await _recomendacionRepository.GetAllAsync();
            return View(recomendaciones.OrderByDescending(r => r.FechaGeneracion));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener recomendaciones");
            TempData["Error"] = "No se pudieron cargar las recomendaciones. Revisa recomendaciones.json.";
            return View(new List<Recomendacion>());
        }
    }

    // POST: Recomendaciones/GenerarTodas
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarTodas()
    {
        try
        {
            _logger.LogInformation("Iniciando generación de recomendaciones para todos los alumnos");
            var generadas = await _recommendationService.GenerarRecomendacionesAsync();
            _logger.LogInformation("Se generaron {Total} recomendaciones", generadas);
            TempData["Success"] = $"Se generaron {generadas} recomendaciones con el motor de inferencia.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar recomendaciones");
            TempData["Error"] = $"No se pudieron generar las recomendaciones: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Recomendaciones/PorAlumno?matricula=ALU-001
    public async Task<IActionResult> PorAlumno(string matricula)
    {
        if (string.IsNullOrEmpty(matricula))
            return NotFound();

        try
        {
            await CargarCatalogosAsync();
            var recomendaciones = await _recomendacionRepository.GetAllAsync();
            var delAlumno = recomendaciones
                .Where(r => r.AlumnoId == matricula)
                .OrderByDescending(r => r.FechaGeneracion)
                .ToList();

            ViewBag.Matricula = matricula;
            if (ViewBag.Alumnos is Dictionary<string, string> nombres && nombres.TryGetValue(matricula, out var nombre))
                ViewBag.NombreAlumno = nombre;

            return View(delAlumno);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener recomendaciones del alumno {Matricula}", matricula);
            TempData["Error"] = "No se pudieron cargar las recomendaciones del alumno.";
            return RedirectToAction(nameof(Index));
        }
    }
}
