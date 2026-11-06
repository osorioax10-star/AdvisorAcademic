using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace SmartAcademicAdvisor.Web.Crud;

/// <summary>Lee y escribe los archivos JSON de la carpeta Data sin depender de los modelos del Core.</summary>
public class JsonFileStore
{
    private readonly string _dir;
    private static readonly object _lock = new();

    public static readonly JsonSerializerOptions Pretty = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public JsonFileStore(string contentRoot) => _dir = Path.Combine(contentRoot, "Data");

    private string PathOf(string file) => Path.Combine(_dir, file);

    public static string Str(JsonObject o, string key) => o[key]?.ToString() ?? "";

    public JsonArray Load(string file)
    {
        lock (_lock)
        {
            var p = PathOf(file);
            if (!File.Exists(p)) return new JsonArray();
            var text = File.ReadAllText(p);
            if (string.IsNullOrWhiteSpace(text)) return new JsonArray();
            return JsonNode.Parse(text) as JsonArray ?? new JsonArray();
        }
    }

    public void Save(string file, JsonArray arr)
    {
        lock (_lock)
        {
            Directory.CreateDirectory(_dir);
            File.WriteAllText(PathOf(file), arr.ToJsonString(Pretty));
        }
    }

    public JsonObject? Find(string file, string id) =>
        Load(file).OfType<JsonObject>().FirstOrDefault(o => string.Equals(Str(o, "id"), id, StringComparison.OrdinalIgnoreCase));

    public string NextId(string file, string prefix)
    {
        int max = 0;
        foreach (var o in Load(file).OfType<JsonObject>())
        {
            var m = Regex.Match(Str(o, "id"), @"(\d+)$");
            if (m.Success && int.TryParse(m.Value, out var n) && n > max) max = n;
        }
        return $"{prefix}-{max + 1:D3}";
    }

    public void Upsert(string file, JsonObject obj)
    {
        var arr = Load(file);
        var id = Str(obj, "id");
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i] is JsonObject o && string.Equals(Str(o, "id"), id, StringComparison.OrdinalIgnoreCase))
            {
                arr[i] = obj;
                Save(file, arr);
                return;
            }
        }
        arr.Add(obj);
        Save(file, arr);
    }

    public bool Delete(string file, string id)
    {
        var arr = Load(file);
        var item = arr.OfType<JsonObject>().FirstOrDefault(o => string.Equals(Str(o, "id"), id, StringComparison.OrdinalIgnoreCase));
        if (item == null) return false;
        arr.Remove(item);
        Save(file, arr);
        return true;
    }
}
