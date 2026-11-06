using SmartAcademicAdvisor.Core.Models;
using SmartAcademicAdvisor.Core.Interfaces;

namespace SmartAcademicAdvisor.Services;

public class EspecialidadService
{
    private readonly IRepository<Especialidad> _repository;

    public EspecialidadService(IRepository<Especialidad> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Especialidad>> ObtenerTodasAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Especialidad?> ObtenerPorIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CrearAsync(Especialidad especialidad)
    {
        await _repository.AddAsync(especialidad);
    }

    public async Task ActualizarAsync(Especialidad especialidad)
    {
        await _repository.UpdateAsync(especialidad);
    }

    public async Task EliminarAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }
}
