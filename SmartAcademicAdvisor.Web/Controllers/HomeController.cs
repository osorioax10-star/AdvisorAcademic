using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmartAcademicAdvisor.Web.Models;
using SmartAcademicAdvisor.Services;
using SmartAcademicAdvisor.Core.Interfaces;
using SmartAcademicAdvisor.Core.Models;

namespace SmartAcademicAdvisor.Web.Controllers;

[SmartAcademicAdvisor.Web.Perfiles.SoloAdmin]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AlumnoService _alumnoService;
    private readonly IRepository<Materia> _materiaRepository;
    private readonly IRepository<Especialidad> _especialidadRepository;
    private readonly IRepository<Recomendacion> _recomendacionRepository;

    public HomeController(
        ILogger<HomeController> logger,
        AlumnoService alumnoService,
        IRepository<Materia> materiaRepository,
        IRepository<Especialidad> especialidadRepository,
        IRepository<Recomendacion> recomendacionRepository)
    {
        _logger = logger;
        _alumnoService = alumnoService;
        _materiaRepository = materiaRepository;
        _especialidadRepository = especialidadRepository;
        _recomendacionRepository = recomendacionRepository;
    }

    /// <summary>
    /// Especialidad de cada alumno: la última recomendación generada por el motor
    /// o, si no hay, la que el alumno ya trae guardada.
    /// </summary>
    private static Dictionary<string, string> EspecialidadPorAlumno(
        List<Alumno> alumnos, List<Recomendacion> recomendaciones, List<Especialidad> especialidades)
    {
        string Nombre(string valor) =>
            especialidades.FirstOrDefault(e => e.Id == valor || e.Nombre == valor)?.Nombre ?? valor;

        var resultado = new Dictionary<string, string>();

        foreach (var a in alumnos.Where(a => !string.IsNullOrWhiteSpace(a.EspecialidadRecomendada)))
            resultado[a.Id] = Nombre(a.EspecialidadRecomendada!);

        foreach (var g in recomendaciones
                     .Where(r => !string.IsNullOrWhiteSpace(r.EspecialidadRecomendada))
                     .GroupBy(r => r.AlumnoId))
        {
            var ultima = g.OrderByDescending(r => r.FechaGeneracion).First();
            resultado[g.Key] = Nombre(ultima.EspecialidadRecomendada!);
        }

        return resultado;
    }

    public async Task<IActionResult> Index()
    {
        // Procesar datos con LINQ para el Dashboard
        var alumnos = (await _alumnoService.ObtenerTodosAsync()).ToList();
        var materias = await _materiaRepository.GetAllAsync();
        var especialidades = (await _especialidadRepository.GetAllAsync()).ToList();
        var recomendaciones = (await _recomendacionRepository.GetAllAsync()).ToList();

        var totalAlumnos = alumnos.Count;

        // Calcular promedio general con Aggregate
        var promedioGeneral = totalAlumnos > 0
            ? alumnos.Select(a => a.Promedio).Aggregate((acc, p) => acc + p) / totalAlumnos
            : 0;

        // Agrupar alumnos por especialidad con GroupBy
        var espPorAlumno = EspecialidadPorAlumno(alumnos, recomendaciones, especialidades);
        var alumnosPorEsp = espPorAlumno.Any()
            ? espPorAlumno.GroupBy(kv => kv.Value).ToDictionary(g => g.Key, g => g.Count())
            : await _alumnoService.AgruparPorEspecialidadAsync();

        // Top 5 intereses con LINQ
        var topIntereses = (await _alumnoService.ObtenerTopInteresesAsync(5))
            .Select(x => new InteresStats
            {
                Interes = x.Interes,
                Cantidad = x.Cantidad,
                Porcentaje = totalAlumnos > 0 ? (x.Cantidad * 100.0 / totalAlumnos) : 0
            })
            .ToList();

        // Especialidades populares
        var colores = new[] { "#059669", "#7c3aed", "#ea580c", "#10b981", "#8b5cf6", "#f97316", "#34d399" };
        var especialidadesPopulares = alumnosPorEsp
            .OrderByDescending(kvp => kvp.Value)
            .Take(7)
            .Select((kvp, index) =>
            {
                var esp = especialidades.FirstOrDefault(e => e.Id == kvp.Key);
                return new EspecialidadStats
                {
                    Nombre = esp?.Nombre ?? kvp.Key,
                    CantidadAlumnos = kvp.Value,
                    Porcentaje = totalAlumnos > 0 ? (kvp.Value * 100.0 / totalAlumnos) : 0,
                    Color = colores[index % colores.Length]
                };
            })
            .ToList();

        // Aprobados vs Reprobados (promedio >= 70 es aprobado)
        var alumnosAprobados = alumnos.Count(a => a.Promedio >= 70);
        var alumnosReprobados = alumnos.Count(a => a.Promedio < 70);
        var porcentajeAprobacion = totalAlumnos > 0 ? (alumnosAprobados * 100.0 / totalAlumnos) : 0;

        // Promedio por especialidad
        var promediosPorEspecialidad = alumnos
            .Where(a => espPorAlumno.ContainsKey(a.Id))
            .GroupBy(a => espPorAlumno[a.Id])
            .Select(g => new PromedioEspecialidadStats
            {
                Especialidad = g.Key,
                PromedioAlumnos = g.Average(a => a.Promedio),
                CantidadAlumnos = g.Count()
            })
            .OrderByDescending(x => x.PromedioAlumnos)
            .ToList();

        var alumnosActivos = alumnos.Count(a => a.Estatus == EstatusAlumno.Activo);

        // ----- Analítica detallada para las gráficas del dashboard (LINQ) -----
        var etiquetasRango = new[] { "Menos de 60", "60 a 69", "70 a 79", "80 a 89", "90 a 100" };
        int Rango(double p) => p < 60 ? 0 : p < 70 ? 1 : p < 80 ? 2 : p < 90 ? 3 : 4;
        var histograma = etiquetasRango.Select((e, i) => new { etiqueta = e, cantidad = alumnos.Count(a => Rango(a.Promedio) == i) }).ToList();

        var porSemestre = alumnos.GroupBy(a => a.Semestre).OrderBy(g => g.Key)
            .Select(g => new
            {
                semestre = g.Key,
                alumnos = g.Count(),
                promedio = Math.Round(g.Average(a => a.Promedio), 1),
                aprobados = g.Count(a => a.Promedio >= 70),
                reprobados = g.Count(a => a.Promedio < 70)
            }).ToList();

        var porEstatus = alumnos.GroupBy(a => a.Estatus.ToString())
            .Select(g => new { etiqueta = g.Key, cantidad = g.Count() })
            .OrderByDescending(x => x.cantidad).ToList();

        var interesesTodos = alumnos.SelectMany(a => a.Intereses).GroupBy(i => i)
            .Select(g => new { etiqueta = g.Key, cantidad = g.Count() })
            .OrderByDescending(x => x.cantidad).ThenBy(x => x.etiqueta).ToList();
        var interesesCalor = interesesTodos.Take(8).Select(x => x.etiqueta).ToList();
        var semestresCalor = Enumerable.Range(1, 10).ToList();
        var calor = new
        {
            intereses = interesesCalor,
            semestres = semestresCalor,
            valores = semestresCalor
                .Select(s => interesesCalor.Select(i => alumnos.Count(a => a.Semestre == s && a.Intereses.Contains(i))).ToList())
                .ToList()
        };

        var dispersion = alumnos.Select(a => new { x = a.Semestre, y = Math.Round(a.Promedio, 1), estatus = a.Estatus.ToString(), nombre = a.Nombre }).ToList();
        var enRiesgo = alumnos.OrderBy(a => a.Promedio).Take(8)
            .Select(a => new { id = a.Id, nombre = a.Nombre, semestre = a.Semestre, promedio = Math.Round(a.Promedio, 1) }).ToList();
        var mejores = alumnos.OrderByDescending(a => a.Promedio).Take(8)
            .Select(a => new { id = a.Id, nombre = a.Nombre, semestre = a.Semestre, promedio = Math.Round(a.Promedio, 1) }).ToList();

        var materiasArea = materias.GroupBy(m => m.Area.ToString())
            .Select(g => new { area = g.Key, cantidad = g.Count(), creditos = g.Sum(m => m.Creditos) })
            .OrderByDescending(x => x.cantidad).ToList();
        var materiasSemestre = materias.GroupBy(m => m.Semestre).OrderBy(g => g.Key)
            .Select(g => new { semestre = g.Key, cantidad = g.Count(), creditos = g.Sum(m => m.Creditos), optativas = g.Count(m => m.EsOptativa) }).ToList();

        var espDetalle = especialidades.OrderBy(e => e.Nombre)
            .Select(e => new
            {
                nombre = e.Nombre,
                promedioMinimo = e.PromedioMinimo,
                pesos = new[] { e.Pesos.PesoPromedio, e.Pesos.PesoIntereses, e.Pesos.PesoMateriasClave, e.Pesos.PesoReglas },
                intereses = e.InteresesClave.Count,
                materias = e.MateriasClave.Count,
                alumnos = espPorAlumno.Values.Count(v => v == e.Nombre)
            }).ToList();
        var distribucion = especialidadesPopulares.Select(e => new { nombre = e.Nombre, cantidad = e.CantidadAlumnos }).ToList();

        ViewBag.DashJson = JsonSerializer.Serialize(new
        {
            kpi = new { total = totalAlumnos, aprobados = alumnosAprobados, reprobados = alumnosReprobados, activos = alumnosActivos },
            histograma, porSemestre, porEstatus, intereses = interesesTodos, calor, dispersion, enRiesgo, mejores,
            materiasArea, materiasSemestre, especialidades = espDetalle, distribucion
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var viewModel = new DashboardViewModel
        {
            TotalAlumnos = totalAlumnos,
            TotalMaterias = materias.Count(),
            TotalEspecialidades = especialidades.Count,
            PromedioGeneral = promedioGeneral,
            AlumnosPorEspecialidad = alumnosPorEsp,
            TopIntereses = topIntereses,
            EspecialidadesPopulares = especialidadesPopulares,
            AlumnosAprobados = alumnosAprobados,
            AlumnosReprobados = alumnosReprobados,
            PorcentajeAprobacion = porcentajeAprobacion,
            PromediosPorEspecialidad = promediosPorEspecialidad,
            AlumnosActivos = alumnosActivos
        };

        return View(viewModel);
    }

    public async Task<IActionResult> IndexNew()
    {
        var alumnos = (await _alumnoService.ObtenerTodosAsync()).ToList();
        var materias = await _materiaRepository.GetAllAsync();
        var especialidades = (await _especialidadRepository.GetAllAsync()).ToList();

        var totalAlumnos = alumnos.Count;
        var promedioGeneral = totalAlumnos > 0
            ? alumnos.Select(a => a.Promedio).Aggregate((acc, p) => acc + p) / totalAlumnos
            : 0;

        var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var alumnosNuevos = alumnos.Count(a => a.FechaRegistro >= inicioMes);
        var alumnosRecientes = alumnos.OrderByDescending(a => a.FechaRegistro).Take(5).ToList();
        var alumnosPorEsp = await _alumnoService.AgruparPorEspecialidadAsync();

        var topIntereses = (await _alumnoService.ObtenerTopInteresesAsync(5))
            .Select(x => new InteresStats
            {
                Interes = x.Interes,
                Cantidad = x.Cantidad,
                Porcentaje = totalAlumnos > 0 ? (x.Cantidad * 100.0 / totalAlumnos) : 0
            })
            .ToList();

        var colores = new[] { "#059669", "#7c3aed", "#ea580c", "#10b981", "#8b5cf6", "#f97316", "#34d399" };
        var especialidadesPopulares = alumnosPorEsp
            .OrderByDescending(kvp => kvp.Value)
            .Take(7)
            .Select((kvp, index) =>
            {
                var esp = especialidades.FirstOrDefault(e => e.Id == kvp.Key);
                return new EspecialidadStats
                {
                    Nombre = esp?.Nombre ?? kvp.Key,
                    CantidadAlumnos = kvp.Value,
                    Porcentaje = totalAlumnos > 0 ? (kvp.Value * 100.0 / totalAlumnos) : 0,
                    Color = colores[index % colores.Length]
                };
            })
            .ToList();

        var viewModel = new DashboardViewModel
        {
            TotalAlumnos = totalAlumnos,
            TotalMaterias = materias.Count(),
            TotalEspecialidades = especialidades.Count,
            PromedioGeneral = promedioGeneral,
            AlumnosNuevos = alumnosNuevos,
            AlumnosRecientes = alumnosRecientes,
            AlumnosPorEspecialidad = alumnosPorEsp,
            TopIntereses = topIntereses,
            EspecialidadesPopulares = especialidadesPopulares
        };

        return View(viewModel);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
