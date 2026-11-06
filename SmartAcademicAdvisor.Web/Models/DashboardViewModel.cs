namespace SmartAcademicAdvisor.Web.Models;

public class DashboardViewModel
{
    public int TotalAlumnos { get; set; }
    public int TotalMaterias { get; set; }
    public int TotalEspecialidades { get; set; }
    public double PromedioGeneral { get; set; }
    public int AlumnosNuevos { get; set; }
    public List<object> AlumnosRecientes { get; set; } = new();
    public Dictionary<string, int> AlumnosPorEspecialidad { get; set; } = new();
    public List<InteresStats> TopIntereses { get; set; } = new();
    public List<EspecialidadStats> EspecialidadesPopulares { get; set; } = new();
    public int AlumnosAprobados { get; set; }
    public int AlumnosReprobados { get; set; }
    public double PorcentajeAprobacion { get; set; }
    public List<PromedioEspecialidadStats> PromediosPorEspecialidad { get; set; } = new();
    public int AlumnosActivos { get; set; }
}

public class InteresStats
{
    public string Interes { get; set; } = "";
    public int Cantidad { get; set; }
    public double Porcentaje { get; set; }
}

public class EspecialidadStats
{
    public string Nombre { get; set; } = "";
    public int CantidadAlumnos { get; set; }
    public double Porcentaje { get; set; }
    public string Color { get; set; } = "";
}

public class PromedioEspecialidadStats
{
    public string Especialidad { get; set; } = "";
    public double PromedioAlumnos { get; set; }
    public int CantidadAlumnos { get; set; }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
