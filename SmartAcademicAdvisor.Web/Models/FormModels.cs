using SmartAcademicAdvisor.Core.Models;

namespace SmartAcademicAdvisor.Web.Models;

public class AlumnoFormModel
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public int Semestre { get; set; }
    public double Promedio { get; set; }
    public List<string> Intereses { get; set; } = new();
    public List<string> InteresesSeleccionados { get; set; } = new();
    public EstatusAlumno Estatus { get; set; }
    public string? EspecialidadRecomendada { get; set; }

    public static List<string> InteresesDisponibles { get; } = new()
    {
        "Programación",
        "Inteligencia Artificial",
        "Desarrollo Web",
        "Desarrollo Móvil",
        "Bases de Datos",
        "Redes",
        "Ciberseguridad",
        "Cloud Computing",
        "DevOps",
        "Machine Learning",
        "Análisis de Datos",
        "Blockchain",
        "IoT",
        "Videojuegos",
        "Realidad Virtual",
        "UI/UX",
        "Big Data",
        "Arquitectura de Software"
    };
}

public class EspecialidadFormModel
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string CampoLaboral { get; set; } = "";
    public double PromedioMinimo { get; set; }
    public List<string> InteresesClave { get; set; } = new();
    public List<string> InteresesSeleccionados { get; set; } = new();
    public List<string> MateriasClave { get; set; } = new();
    public List<string> MateriasClaveIds { get; set; } = new();
    public string CompetenciasRequeridas { get; set; } = "";
    public PesosAfinidad Pesos { get; set; } = new();
    
    public double PesoPromedio { get; set; }
    public double PesoIntereses { get; set; }
    public double PesoMateriasClave { get; set; }
    public double PesoReglas { get; set; }
}

public class MateriaFormModel
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Semestre { get; set; }
    public int Creditos { get; set; }
    public AreaMateria Area { get; set; }
    public bool EsOptativa { get; set; }
    public List<string> PrerrequisitoIds { get; set; } = new();
    public List<string> EspecialidadIds { get; set; } = new();
}
