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
}
