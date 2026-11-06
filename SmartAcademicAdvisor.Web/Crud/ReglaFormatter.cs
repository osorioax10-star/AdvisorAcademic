using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace SmartAcademicAdvisor.Web.Crud;

/// <summary>Convierte una regla JSON en código lógico (cabeza :- cuerpo) con colores de sintaxis.</summary>
public static class ReglaFormatter
{
    private static string E(string s) => WebUtility.HtmlEncode(s);

    private static string Simbolo(string op) => op switch
    {
        "MayorIgual" => "≥",
        "Mayor" => ">",
        "Menor" => "<",
        "MenorIgual" => "≤",
        "Igual" => "=",
        "Diferente" => "≠",
        _ => op
    };

    private static string Arg(JsonNode? n)
    {
        var s = n?.ToString() ?? "";
        if (s.Length == 1 && char.IsUpper(s[0])) return "<span class=\"k-var\">" + E(s) + "</span>";
        if (s.StartsWith("ESP-") || s.StartsWith("MAT-") || s.StartsWith("CERT-")) return "<span class=\"k-id\">" + E(s) + "</span>";
        return "<span class=\"k-str\">\"" + E(s) + "\"</span>";
    }

    public static string Atomo(JsonObject o)
    {
        var nombre = JsonFileStore.Str(o, o.ContainsKey("predicado") ? "predicado" : "nombre");
        var args = (o["argumentos"] as JsonArray)?.Select(a => Arg(a)) ?? Enumerable.Empty<string>();

        var sb = new StringBuilder();
        sb.Append("<span class=\"k-pred\">").Append(E(nombre)).Append("</span>");
        sb.Append("<span class=\"k-punc\">(</span>");
        sb.Append(string.Join("<span class=\"k-punc\">, </span>", args));
        sb.Append("<span class=\"k-punc\">)</span>");

        var op = JsonFileStore.Str(o, "operador");
        var val = o["valorComparacion"]?.ToString();
        if (op != "" && val != null)
            sb.Append(" <span class=\"k-op\">").Append(Simbolo(op)).Append("</span> <span class=\"k-num\">").Append(E(val)).Append("</span>");
        return sb.ToString();
    }

    public static string Codigo(JsonObject cabeza, IList<JsonObject> cuerpo)
    {
        var sb = new StringBuilder();
        sb.Append(Atomo(cabeza)).Append(" <span class=\"k-neck\">:-</span>");
        for (int i = 0; i < cuerpo.Count; i++)
        {
            sb.Append('\n').Append("    ").Append(Atomo(cuerpo[i]));
            sb.Append("<span class=\"k-punc\">").Append(i < cuerpo.Count - 1 ? "," : ".").Append("</span>");
        }
        return sb.ToString();
    }

    /// <summary>Id de la especialidad que concluye la regla (primer argumento ESP-xxx de la cabeza).</summary>
    public static string Destino(JsonObject regla)
    {
        var args = (regla["cabeza"]?["argumentos"] as JsonArray)?.Select(a => a?.ToString() ?? "") ?? Enumerable.Empty<string>();
        return args.FirstOrDefault(a => a.StartsWith("ESP-")) ?? "";
    }
}
