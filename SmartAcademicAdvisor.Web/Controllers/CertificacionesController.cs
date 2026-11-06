using SmartAcademicAdvisor.Web.Crud;

namespace SmartAcademicAdvisor.Web.Controllers;

public class CertificacionesController : JsonCrudController
{
    public CertificacionesController(IWebHostEnvironment env) : base(env) { }
    protected override CrudSchema Schema => CrudSchemas.Certificaciones;
}
