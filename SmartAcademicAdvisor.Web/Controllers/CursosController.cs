using SmartAcademicAdvisor.Web.Crud;

namespace SmartAcademicAdvisor.Web.Controllers;

public class CursosController : JsonCrudController
{
    public CursosController(IWebHostEnvironment env) : base(env) { }
    protected override CrudSchema Schema => CrudSchemas.Cursos;
}
