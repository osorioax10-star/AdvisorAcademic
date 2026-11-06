using System.Globalization;
using System.Text.Json.Nodes;
using SmartAcademicAdvisor.Web.Crud;

namespace SmartAcademicAdvisor.Web.Controllers;

public class HistorialController : JsonCrudController
{
    public HistorialController(IWebHostEnvironment env) : base(env) { }
    protected override CrudSchema Schema => CrudSchemas.Historial;

    // RF-06: el resultado (aprobado o no) se calcula con la calificación mínima aprobatoria
    protected override void Transform(JsonObject obj, bool isNew)
    {
        if (double.TryParse(obj["calificacion"]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var cal))
            obj["estatus"] = cal >= 70 ? 0 : 1;
        if (isNew) obj["fechaRegistro"] = DateTime.Now.ToString("s");
    }
}
