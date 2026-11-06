using SmartAcademicAdvisor.Core.Models;
using SmartAcademicAdvisor.Core.Interfaces;

namespace SmartAcademicAdvisor.Services;

public class AlumnoService
{
    private readonly IRepository<Alumno> _repository;

    public AlumnoService(IRepository<Alumno> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Alumno>> ObtenerTodosAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Dictionary<string, int>> AgruparPorEspecialidadAsync()
    {
        var alumnos = await _repository.GetAllAsync();
        return alumnos
            .Where(a => !string.IsNullOrWhiteSpace(a.EspecialidadRecomendada))
            .GroupBy(a => a.EspecialidadRecomendada!)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<List<(string Interes, int Cantidad)>> ObtenerTopInteresesAsync(int top)
    {
        var alumnos = await _repository.GetAllAsync();
        return alumnos
            .SelectMany(a => a.Intereses)
            .GroupBy(i => i)
            .OrderByDescending(g => g.Count())
            .Take(top)
            .Select(g => (g.Key, g.Count()))
            .ToList();
    }
}
