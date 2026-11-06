namespace SmartAcademicAdvisor.Web.Crud;

public static class CrudSchemas
{
    private static readonly List<Opt> NivelesCurso = new()
    {
        new("0", "Básico"), new("1", "Intermedio"), new("2", "Avanzado")
    };

    private static readonly List<Opt> NivelesCert = new()
    {
        new("0", "Fundamental"), new("1", "Asociado"), new("2", "Especialista"), new("3", "Profesional")
    };

    public static readonly CrudSchema Cursos = new()
    {
        Title = "Cursos complementarios",
        Subtitle = "Catálogo de cursos que el motor puede recomendar",
        File = "cursos.json", Prefix = "CUR", Singular = "curso",
        Cols = new()
        {
            new("nombre", "Curso"), new("proveedor", "Proveedor"), new("horasDuracion", "Horas", ColKind.Mono),
            new("nivel", "Nivel", ColKind.Label), new("costo", "Costo", ColKind.Money)
        },
        Fields = new()
        {
            new() { Key = "nombre", Label = "Nombre del curso", Required = true },
            new() { Key = "proveedor", Label = "Proveedor / plataforma", Required = true, Width = 6 },
            new() { Key = "horasDuracion", Label = "Duración (horas)", Kind = FieldKind.Number, Required = true, Min = 1, Max = 2000, Width = 3 },
            new() { Key = "nivel", Label = "Nivel", Kind = FieldKind.Select, Required = true, AsNumber = true, Options = NivelesCurso, Width = 3 },
            new() { Key = "costo", Label = "Costo (MXN)", Kind = FieldKind.Number, Min = 0, Default = "0", Width = 4 },
            new() { Key = "esGratuito", Label = "Es gratuito", Kind = FieldKind.Bool, Width = 8 },
            new() { Key = "url", Label = "Enlace del curso" },
            new() { Key = "descripcion", Label = "Descripción", Kind = FieldKind.TextArea },
            new() { Key = "especialidadesRelacionadas", Label = "Especialidades relacionadas", Kind = FieldKind.Multi, OptionsFile = "especialidades.json" }
        }
    };

    public static readonly CrudSchema Certificaciones = new()
    {
        Title = "Certificaciones",
        Subtitle = "Catálogo de certificaciones profesionales que el motor puede recomendar",
        File = "certificaciones.json", Prefix = "CERT", Singular = "certificación",
        Cols = new()
        {
            new("nombre", "Certificación"), new("proveedor", "Proveedor"), new("nivel", "Nivel", ColKind.Label),
            new("validezAnios", "Validez (años)", ColKind.Mono), new("costo", "Costo", ColKind.Money)
        },
        Fields = new()
        {
            new() { Key = "nombre", Label = "Nombre de la certificación", Required = true },
            new() { Key = "proveedor", Label = "Proveedor", Required = true, Width = 6 },
            new() { Key = "nivel", Label = "Nivel", Kind = FieldKind.Select, Required = true, AsNumber = true, Options = NivelesCert, Width = 3 },
            new() { Key = "validezAnios", Label = "Validez (años)", Kind = FieldKind.Number, Min = 0, Max = 20, Width = 3 },
            new() { Key = "costo", Label = "Costo (USD)", Kind = FieldKind.Number, Min = 0, Width = 4 },
            new() { Key = "urlInformacion", Label = "Enlace de información", Width = 8 },
            new() { Key = "descripcion", Label = "Descripción", Kind = FieldKind.TextArea },
            new() { Key = "prerequisitosRecomendados", Label = "Prerrequisitos recomendados", Kind = FieldKind.Lines, Help = "Uno por línea" },
            new() { Key = "especialidadesRelacionadas", Label = "Especialidades relacionadas", Kind = FieldKind.Multi, OptionsFile = "especialidades.json" }
        }
    };

    public static readonly CrudSchema Reglas = new()
    {
        Title = "Motor de inferencia · Reglas",
        Subtitle = "Reglas de programación lógica (cabeza :- cuerpo) que evalúa el encadenamiento hacia adelante",
        File = "reglas.json", Prefix = "R", Singular = "regla", ManualId = true,
        Cols = new()
        {
            new("id", "ID", ColKind.Mono), new("nombre", "Regla"), new("peso", "Peso", ColKind.Percent),
            new("cuerpo", "Premisas", ColKind.Count), new("activa", "Estado", ColKind.Bool)
        },
        Fields = new()
        {
            new() { Key = "nombre", Label = "Nombre de la regla", Required = true, Width = 8 },
            new() { Key = "peso", Label = "Peso (0 a 1)", Kind = FieldKind.Number, Required = true, Min = 0, Max = 1, Default = "0.4", Width = 4 },
            new() { Key = "descripcion", Label = "Descripción", Kind = FieldKind.TextArea, Help = "Explica con palabras qué hace la regla; es lo que verá el alumno como justificación." },
            new() { Key = "activa", Label = "Regla activa", Kind = FieldKind.Bool, Default = "true" },
            new()
            {
                Key = "cabeza", Label = "Cabeza (conclusión)", Kind = FieldKind.Json, Required = true,
                Help = "Predicado que se infiere. Ej.: EspecialidadRecomendada(X, ESP-003)",
                Default = """
                {
                  "nombre": "EspecialidadRecomendada",
                  "argumentos": ["X", "ESP-003"]
                }
                """
            },
            new()
            {
                Key = "cuerpo", Label = "Cuerpo (premisas)", Kind = FieldKind.Json, Required = true,
                Help = "Lista de condiciones. Predicados disponibles: Promedio, TieneInteres, MateriaAprobada. Operadores: MayorIgual, Mayor, Menor, MenorIgual, Igual.",
                Default = """
                [
                  { "predicado": "Promedio", "argumentos": ["X", "P"], "operador": "MayorIgual", "valorComparacion": 85.0 },
                  { "predicado": "TieneInteres", "argumentos": ["X", "Inteligencia Artificial"], "operador": null, "valorComparacion": null }
                ]
                """
            }
        }
    };

    public static readonly CrudSchema Historial = new()
    {
        Title = "Historial académico",
        Subtitle = "Calificaciones por alumno y materia. El resultado se calcula con aprobatoria de 70",
        File = "historial.json", Prefix = "HIST", Singular = "registro", NameKey = "id",
        Cols = new()
        {
            new("id", "ID", ColKind.Mono), new("alumnoId", "Alumno", ColKind.Label), new("materiaId", "Materia", ColKind.Label),
            new("calificacion", "Calificación", ColKind.Mono), new("periodo", "Periodo", ColKind.Mono), new("calificacion", "Resultado", ColKind.Pass)
        },
        Fields = new()
        {
            new() { Key = "alumnoId", Label = "Alumno", Kind = FieldKind.Select, Required = true, OptionsFile = "alumnos.json", Width = 6 },
            new() { Key = "materiaId", Label = "Materia", Kind = FieldKind.Select, Required = true, OptionsFile = "materias.json", Width = 6 },
            new() { Key = "calificacion", Label = "Calificación (0 a 100)", Kind = FieldKind.Number, Required = true, Min = 0, Max = 100, Width = 6 },
            new() { Key = "periodo", Label = "Periodo", Required = true, Default = "2026-1", Help = "Formato año-ciclo, por ejemplo 2026-1", Width = 6 }
        }
    };
}
