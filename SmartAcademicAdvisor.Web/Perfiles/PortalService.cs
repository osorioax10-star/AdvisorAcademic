using System.Globalization;
using System.Text.Json.Nodes;
using SmartAcademicAdvisor.Web.Crud;

namespace SmartAcademicAdvisor.Web.Perfiles;

/// <summary>
/// Calcula todo lo que ve el alumno leyendo los JSON. La afinidad usa los pesos propios de cada
/// especialidad y evalúa las reglas del motor (Promedio, TieneInteres, MateriaAprobada) para poder explicar el porqué.
/// </summary>
public static class PortalService
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly string[] Areas =
    {
        "Programación", "Bases de datos", "Redes", "Matemáticas aplicadas", "Inteligencia artificial",
        "Seguridad", "Ingeniería de software", "Sistemas operativos", "Arquitectura de computadoras", "Desarrollo web"
    };
    private static readonly string[] NivelesCurso = { "Básico", "Intermedio", "Avanzado" };

    private static double D(JsonNode? n, double def = 0) =>
        n != null && double.TryParse(n.ToString(), NumberStyles.Float, Inv, out var v) ? v : def;
    private static string S(JsonNode? n) => n?.ToString() ?? "";
    private static List<string> L(JsonNode? n) => (n as JsonArray)?.Select(x => x?.ToString() ?? "").ToList() ?? new List<string>();

    private static string Prop(JsonObject o, string nombre)
    {
        foreach (var kv in o)
            if (string.Equals(kv.Key, nombre, StringComparison.OrdinalIgnoreCase)) return kv.Value?.ToString() ?? "";
        return "";
    }

    private static string Area(int i) => i >= 0 && i < Areas.Length ? Areas[i] : "General";

    private static string EstatusTexto(JsonNode? n) => S(n) switch
    {
        "0" or "" => "Activo",
        "1" => "Inactivo",
        "2" => "Egresado",
        "3" => "Baja temporal",
        var otro => otro
    };

    private static string Simbolo(string op) => op switch
    {
        "MayorIgual" => "≥", "Mayor" => ">", "Menor" => "<", "MenorIgual" => "≤", "Igual" => "=", "Diferente" => "≠", _ => "≥"
    };

    private static bool Comparar(double a, string op, double b) => op switch
    {
        "MayorIgual" => a >= b, "Mayor" => a > b, "Menor" => a < b, "MenorIgual" => a <= b,
        "Igual" => Math.Abs(a - b) < 0.01, "Diferente" => Math.Abs(a - b) >= 0.01, _ => a >= b
    };

    private static PremisaEval? EvaluarPremisa(JsonObject p, PortalData d, Dictionary<string, double> aprobadas, Dictionary<string, string> nombres)
    {
        var predicado = S(p["predicado"]);
        var args = L(p["argumentos"]);
        var op = S(p["operador"]);

        if (predicado == "Promedio")
        {
            var v = D(p["valorComparacion"]);
            return new PremisaEval
            {
                Texto = "Promedio " + Simbolo(op) + " " + v.ToString("0.#", Inv),
                Detalle = "tienes " + d.Promedio.ToString("0.0", Inv),
                Cumple = Comparar(d.Promedio, op, v)
            };
        }
        if (predicado == "TieneInteres")
        {
            var interes = args.Count > 1 ? args[1] : "";
            var ok = d.Intereses.Any(x => string.Equals(x, interes, StringComparison.OrdinalIgnoreCase));
            return new PremisaEval { Texto = "Interés en " + interes, Detalle = ok ? "lo declaraste" : "no lo tienes declarado", Cumple = ok };
        }
        if (predicado == "MateriaAprobada")
        {
            var id = args.Count > 1 ? args[1] : "";
            var ok = aprobadas.ContainsKey(id);
            var nombre = nombres.TryGetValue(id, out var nm) ? nm : id;
            return new PremisaEval
            {
                Texto = "Materia aprobada: " + nombre,
                Detalle = ok ? "calificación " + aprobadas[id].ToString("0.#", Inv) : "aún no la apruebas",
                Cumple = ok
            };
        }
        return null;
    }

    private static List<ItemCat> Items(List<JsonObject> origen, string espId, bool esCurso) =>
        origen.Where(c => L(c["especialidadesRelacionadas"]).Contains(espId))
            .Select(c =>
            {
                int nivel = (int)D(c["nivel"]);
                return new ItemCat
                {
                    Id = S(c["id"]),
                    Nombre = S(c["nombre"]),
                    Proveedor = S(c["proveedor"]),
                    Costo = D(c["costo"]),
                    Url = S(esCurso ? c["url"] : c["urlInformacion"]),
                    Detalle = esCurso
                        ? D(c["horasDuracion"]).ToString("0", Inv) + " h · " + (nivel >= 0 && nivel < NivelesCurso.Length ? NivelesCurso[nivel] : "")
                        : "Vigencia de " + D(c["validezAnios"]).ToString("0", Inv) + " años"
                };
            })
            .OrderBy(c => c.Costo).ThenBy(c => c.Nombre).Take(esCurso ? 4 : 3).ToList();

    public static PortalData? Construir(JsonFileStore st, string alumnoId)
    {
        var alumnos = st.Load("alumnos.json").OfType<JsonObject>().ToList();
        var a = alumnos.FirstOrDefault(x => string.Equals(S(x["id"]), alumnoId, StringComparison.OrdinalIgnoreCase));
        if (a == null) return null;

        var d = new PortalData
        {
            Id = S(a["id"]), Nombre = S(a["nombre"]), Correo = S(a["correo"]), Semestre = (int)D(a["semestre"]),
            Promedio = D(a["promedio"]), Estatus = EstatusTexto(a["estatus"]), Intereses = L(a["intereses"]), TotalAlumnos = alumnos.Count
        };
        d.Posicion = alumnos.Count(x => D(x["promedio"]) > d.Promedio) + 1;
        d.Percentil = alumnos.Count > 1 ? alumnos.Count(x => D(x["promedio"]) < d.Promedio) * 100.0 / (alumnos.Count - 1) : 100;

        // ----- catálogo de materias -----
        var catalogo = st.Load("materias.json").OfType<JsonObject>().Select(m => new MateriaEst
        {
            Id = S(m["id"]), Nombre = S(m["nombre"]), Semestre = (int)D(m["semestre"]), Creditos = (int)D(m["creditos"]),
            Optativa = string.Equals(S(m["esOptativa"]), "true", StringComparison.OrdinalIgnoreCase),
            Area = Area((int)D(m["area"])), Prerreq = L(m["prerrequisitos"]), Especialidades = L(m["especialidadesRelacionadas"])
        }).OrderBy(m => m.Semestre).ThenBy(m => m.Nombre).ToList();
        var nombres = catalogo.GroupBy(m => m.Id).ToDictionary(g => g.Key, g => g.First().Nombre);

        // ----- historial del alumno -----
        d.Historial = st.Load("historial.json").OfType<JsonObject>()
            .Where(h => string.Equals(S(h["alumnoId"]), d.Id, StringComparison.OrdinalIgnoreCase))
            .Select(h => new RegistroNota
            {
                MateriaId = S(h["materiaId"]),
                Materia = nombres.TryGetValue(S(h["materiaId"]), out var nm) ? nm : S(h["materiaId"]),
                Periodo = S(h["periodo"]), Calificacion = D(h["calificacion"]), Aprobada = D(h["calificacion"]) >= 70
            }).OrderBy(n => n.Periodo).ThenBy(n => n.Materia).ToList();

        var aprobadas = d.Historial.Where(n => n.Aprobada).GroupBy(n => n.MateriaId).ToDictionary(g => g.Key, g => g.Max(n => n.Calificacion));
        d.Periodos = d.Historial.GroupBy(n => n.Periodo).OrderBy(g => g.Key)
            .Select(g => new PeriodoProm { Periodo = g.Key, Promedio = Math.Round(g.Average(n => n.Calificacion), 1), Cantidad = g.Count() }).ToList();

        foreach (var m in catalogo)
        {
            m.PrerreqNombres = m.Prerreq.Select(p => nombres.TryGetValue(p, out var pn) ? pn : p).ToList();
            if (aprobadas.TryGetValue(m.Id, out var cal)) { m.Estado = "aprobada"; m.Calificacion = cal; }
            else m.Estado = m.Prerreq.All(p => aprobadas.ContainsKey(p)) ? "disponible" : "bloqueada";
        }
        d.Materias = catalogo;
        d.MateriasAprobadas = catalogo.Count(m => m.Estado == "aprobada");
        d.MateriasTotal = catalogo.Count;
        d.CreditosAprobados = catalogo.Where(m => m.Estado == "aprobada").Sum(m => m.Creditos);
        d.CreditosTotal = catalogo.Sum(m => m.Creditos);

        // ----- afinidad con cada especialidad -----
        var cursos = st.Load("cursos.json").OfType<JsonObject>().ToList();
        var certs = st.Load("certificaciones.json").OfType<JsonObject>().ToList();
        var reglas = st.Load("reglas.json").OfType<JsonObject>()
            .Where(r => !string.Equals(S(r["activa"]), "false", StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var e in st.Load("especialidades.json").OfType<JsonObject>())
        {
            var id = S(e["id"]);
            var pesos = e["pesos"] as JsonObject;
            double wP = D(pesos?["pesoPromedio"], .25), wI = D(pesos?["pesoIntereses"], .30), wM = D(pesos?["pesoMateriasClave"], .25), wR = D(pesos?["pesoReglas"], .20);
            double min = D(e["promedioMinimo"]);
            var clave = L(e["interesesClave"]);
            var mclave = L(e["materiasClave"]);

            var coinciden = clave.Where(c => d.Intereses.Any(i => string.Equals(i, c, StringComparison.OrdinalIgnoreCase))).ToList();
            var mOk = mclave.Where(x => aprobadas.ContainsKey(x)).ToList();

            double sP = min <= 0 ? 100 : Math.Min(100, d.Promedio / min * 100);
            double sI = clave.Count == 0 ? 0 : coinciden.Count * 100.0 / clave.Count;
            double sM = mclave.Count == 0 ? 0 : mOk.Count * 100.0 / mclave.Count;

            var evals = new List<ReglaEval>();
            foreach (var r in reglas)
            {
                var cabeza = r["cabeza"] as JsonObject;
                if (cabeza == null || S(cabeza["nombre"]) != "EspecialidadRecomendada" || !L(cabeza["argumentos"]).Contains(id)) continue;
                var re = new ReglaEval { Id = S(r["id"]), Nombre = S(r["nombre"]), Descripcion = S(r["descripcion"]), Peso = D(r["peso"]) };
                foreach (var prem in (r["cuerpo"] as JsonArray)?.OfType<JsonObject>() ?? Enumerable.Empty<JsonObject>())
                {
                    var pe = EvaluarPremisa(prem, d, aprobadas, nombres);
                    if (pe != null) re.Premisas.Add(pe);
                }
                re.Fraccion = re.Premisas.Count == 0 ? 0 : re.Premisas.Count(x => x.Cumple) / (double)re.Premisas.Count;
                re.Cumplida = re.Premisas.Count > 0 && re.Premisas.All(x => x.Cumple);
                evals.Add(re);
            }
            double sR = evals.Count == 0 ? 0 : evals.Max(x => x.Fraccion) * 100;
            if (evals.Count == 0) wR = 0;                               // sin reglas, ese peso no cuenta
            double suma = wP + wI + wM + wR;
            if (suma <= 0) { wP = wI = wM = .25; suma = .75; }

            var af = new AfinidadEsp
            {
                Id = id, Nombre = S(e["nombre"]), Descripcion = S(e["descripcion"]), CampoLaboral = S(e["campoLaboral"]), PromedioMinimo = min,
                SP = sP, SI = sI, SM = sM, SR = sR, WP = wP, WI = wI, WM = wM, WR = wR,
                Afinidad = (sP * wP + sI * wI + sM * wM + sR * wR) / suma,
                Competencias = L(e["competenciasRequeridas"]),
                InteresesCoinciden = coinciden, InteresesFaltan = clave.Except(coinciden).ToList(),
                MateriasClaveOk = mOk.Select(x => nombres.TryGetValue(x, out var n1) ? n1 : x).ToList(),
                MateriasClavePendientes = mclave.Except(mOk).Select(x => nombres.TryGetValue(x, out var n2) ? n2 : x).ToList(),
                Reglas = evals.OrderByDescending(x => x.Fraccion).ToList()
            };

            var mejorRegla = af.Reglas.FirstOrDefault();
            if (mejorRegla != null)
                af.Faltan.AddRange(mejorRegla.Premisas.Where(x => !x.Cumple).Select(x => x.Texto + " (" + x.Detalle + ")"));
            else if (d.Promedio < min)
                af.Faltan.Add("Promedio mínimo " + min.ToString("0", Inv) + " (tienes " + d.Promedio.ToString("0.0", Inv) + ")");

            bool relacionada(MateriaEst m) => m.Especialidades.Contains(id) || mclave.Contains(m.Id);
            af.Sugeridas = catalogo.Where(m => m.Estado == "disponible" && relacionada(m)).OrderBy(m => m.Semestre).Take(6).ToList();
            af.Siguientes = catalogo.Where(m => m.Estado == "bloqueada" && relacionada(m)).OrderBy(m => m.Semestre).Take(4).ToList();
            af.Cursos = Items(cursos, id, true);
            af.Certificaciones = Items(certs, id, false);
            d.Afinidades.Add(af);
        }
        d.Afinidades = d.Afinidades.OrderByDescending(x => x.Afinidad).ToList();

        // ----- última recomendación oficial del motor (si existe) -----
        var rec = st.Load("recomendaciones.json").OfType<JsonObject>()
            .Where(r => string.Equals(Prop(r, "AlumnoId"), d.Id, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => Prop(r, "FechaGeneracion")).FirstOrDefault();
        if (rec != null)
        {
            var fecha = Prop(rec, "FechaGeneracion");
            d.Oficial = new OficialRec
            {
                Especialidad = Prop(rec, "EspecialidadRecomendada"),
                Fecha = DateTime.TryParse(fecha, Inv, DateTimeStyles.None, out var f) ? f.ToString("dd/MM/yyyy") : fecha
            };
        }
        return d;
    }
}
