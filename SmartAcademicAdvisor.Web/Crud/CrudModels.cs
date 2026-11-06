using System.Text.Json.Nodes;

namespace SmartAcademicAdvisor.Web.Crud;

public enum FieldKind { Text, TextArea, Number, Bool, Select, Multi, Lines, Json }
public enum ColKind { Text, Mono, Bool, Label, Count, Money, Percent, Pass }

public record Opt(string Value, string Label);
public record Col(string Key, string Label, ColKind Kind = ColKind.Text);

public class Field
{
    public string Key { get; init; } = "";
    public string Label { get; init; } = "";
    public FieldKind Kind { get; init; } = FieldKind.Text;
    public bool Required { get; init; }
    public bool AsNumber { get; init; }          // Select cuyo valor se guarda como número
    public string? Help { get; init; }
    public string? Default { get; init; }
    public double? Min { get; init; }
    public double? Max { get; init; }
    public List<Opt> Options { get; init; } = new();
    public string? OptionsFile { get; init; }    // llena las opciones desde otro JSON
    public string OptionsValue { get; init; } = "id";
    public string OptionsLabel { get; init; } = "nombre";
    public int Width { get; init; } = 12;        // 1 a 12
}

public class CrudSchema
{
    public string Title { get; init; } = "";
    public string Subtitle { get; init; } = "";
    public string File { get; init; } = "";
    public string Prefix { get; init; } = "ID";
    public string Singular { get; init; } = "registro";
    public string NameKey { get; init; } = "nombre";
    public bool ManualId { get; init; }
    public List<Col> Cols { get; init; } = new();
    public List<Field> Fields { get; init; } = new();
}

public class CrudIndexVm
{
    public CrudSchema Schema { get; init; } = null!;
    public List<JsonObject> Rows { get; init; } = new();
    public Dictionary<string, List<Opt>> Options { get; init; } = new();
}

public class CrudFormVm
{
    public CrudSchema Schema { get; init; } = null!;
    public bool IsEdit { get; init; }
    public string Id { get; set; } = "";
    public Dictionary<string, string> Values { get; } = new();
    public Dictionary<string, HashSet<string>> Selected { get; } = new();
    public Dictionary<string, List<Opt>> Options { get; init; } = new();
    public Dictionary<string, string> Errors { get; init; } = new();
}

public class CrudDeleteVm
{
    public CrudSchema Schema { get; init; } = null!;
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
}
