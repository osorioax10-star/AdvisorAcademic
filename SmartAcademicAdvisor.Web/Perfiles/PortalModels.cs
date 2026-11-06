namespace SmartAcademicAdvisor.Web.Perfiles;

public class PremisaEval { public string Texto { get; set; } = ""; public string Detalle { get; set; } = ""; public bool Cumple { get; set; } }

public class ReglaEval
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public double Peso { get; set; }
    public double Fraccion { get; set; }
    public bool Cumplida { get; set; }
    public List<PremisaEval> Premisas { get; set; } = new();
}

public class ItemCat
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Proveedor { get; set; } = "";
    public string Detalle { get; set; } = "";
    public double Costo { get; set; }
    public string Url { get; set; } = "";
}

public class MateriaEst
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Semestre { get; set; }
    public int Creditos { get; set; }
    public bool Optativa { get; set; }
    public string Area { get; set; } = "";
    public string Estado { get; set; } = "bloqueada";     // aprobada | disponible | bloqueada
    public double? Calificacion { get; set; }
    public List<string> Prerreq { get; set; } = new();
    public List<string> PrerreqNombres { get; set; } = new();
    public List<string> Especialidades { get; set; } = new();
}

public class AfinidadEsp
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string CampoLaboral { get; set; } = "";
    public double PromedioMinimo { get; set; }
    public double Afinidad { get; set; }
    public double SP { get; set; }
    public double SI { get; set; }
    public double SM { get; set; }
    public double SR { get; set; }
    public double WP { get; set; }
    public double WI { get; set; }
    public double WM { get; set; }
    public double WR { get; set; }
    public List<string> Competencias { get; set; } = new();
    public List<string> InteresesCoinciden { get; set; } = new();
    public List<string> InteresesFaltan { get; set; } = new();
    public List<string> MateriasClaveOk { get; set; } = new();
    public List<string> MateriasClavePendientes { get; set; } = new();
    public List<ReglaEval> Reglas { get; set; } = new();
    public List<string> Faltan { get; set; } = new();
    public List<MateriaEst> Sugeridas { get; set; } = new();
    public List<MateriaEst> Siguientes { get; set; } = new();
    public List<ItemCat> Cursos { get; set; } = new();
    public List<ItemCat> Certificaciones { get; set; } = new();
}

public class RegistroNota
{
    public string MateriaId { get; set; } = "";
    public string Materia { get; set; } = "";
    public string Periodo { get; set; } = "";
    public double Calificacion { get; set; }
    public bool Aprobada { get; set; }
}

public class PeriodoProm { public string Periodo { get; set; } = ""; public double Promedio { get; set; } public int Cantidad { get; set; } }

public class OficialRec { public string Especialidad { get; set; } = ""; public string Fecha { get; set; } = ""; }

public class PortalData
{
    public string Id { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Estatus { get; set; } = "";
    public int Semestre { get; set; }
    public double Promedio { get; set; }
    public List<string> Intereses { get; set; } = new();
    public int TotalAlumnos { get; set; }
    public int Posicion { get; set; }
    public double Percentil { get; set; }
    public int MateriasAprobadas { get; set; }
    public int MateriasTotal { get; set; }
    public int CreditosAprobados { get; set; }
    public int CreditosTotal { get; set; }
    public List<PeriodoProm> Periodos { get; set; } = new();
    public List<RegistroNota> Historial { get; set; } = new();
    public List<MateriaEst> Materias { get; set; } = new();
    public List<AfinidadEsp> Afinidades { get; set; } = new();
    public OficialRec? Oficial { get; set; }
    public AfinidadEsp? Mejor => Afinidades.FirstOrDefault();
}
