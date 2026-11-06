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
    public EstatusAlumno Estatus { get; set; }
    public string? EspecialidadRecomendada { get; set; }
}

public class EspecialidadFormModel
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string CampoLaboral { get; set; } = "";
    public double PromedioMinimo { get; set; }
    public List<string> InteresesClave { get; set; } = new();
    public List<string> MateriasClave { get; set; } = new();
    public PesosAfinidad Pesos { get; set; } = new();
}

public class MateriaFormModel
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Semestre { get; set; }
    public int Creditos { get; set; }
    public AreaMateria Area { get; set; }
    public bool EsOptativa { get; set; }
}
