using SmartAcademicAdvisor.Core.Models;
using SmartAcademicAdvisor.Core.Interfaces;

namespace SmartAcademicAdvisor.Services;

public class MateriaService
{
    private readonly IRepository<Materia> _repository;

    public MateriaService(IRepository<Materia> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Materia>> ObtenerTodasAsync()
    {
        return await _repository.GetAllAsync();
    }
}
