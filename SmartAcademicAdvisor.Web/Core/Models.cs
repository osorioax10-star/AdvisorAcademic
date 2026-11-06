namespace SmartAcademicAdvisor.Core.Models;

public class Alumno
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public int Semestre { get; set; }
    public double Promedio { get; set; }
    public List<string> Intereses { get; set; } = new();
    public EstatusAlumno Estatus { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string? EspecialidadRecomendada { get; set; }
}

public enum EstatusAlumno
{
    Activo = 0,
    Egresado = 1,
    Baja = 2,
    Intercambio = 3
}

public class Materia
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Semestre { get; set; }
    public int Creditos { get; set; }
    public AreaMateria Area { get; set; }
    public bool EsOptativa { get; set; }
}

public enum AreaMateria
{
    Programacion,
    Matematicas,
    Redes,
    BaseDeDatos,
    Hardware,
    Otros
}

public class Especialidad
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string CampoLaboral { get; set; } = "";
    public double PromedioMinimo { get; set; }
    public PesosAfinidad Pesos { get; set; } = new();
    public List<string> InteresesClave { get; set; } = new();
    public List<string> MateriasClave { get; set; } = new();
    public List<string> CompetenciasRequeridas { get; set; } = new();
}

public class PesosAfinidad
{
    public double PesoPromedio { get; set; }
    public double PesoIntereses { get; set; }
    public double PesoMateriasClave { get; set; }
    public double PesoReglas { get; set; }
}

public class Recomendacion
{
    public string Id { get; set; } = "";
    public string AlumnoId { get; set; } = "";
    public string? EspecialidadRecomendada { get; set; }
    public double Afinidad { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public string Justificacion { get; set; } = "";
    public Dictionary<string, double> Afinidades { get; set; } = new();
    public List<string> MateriasRecomendadas { get; set; } = new();
    public List<string> CertificacionesRecomendadas { get; set; } = new();
}
