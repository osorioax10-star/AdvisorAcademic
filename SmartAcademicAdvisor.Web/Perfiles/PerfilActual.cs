using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartAcademicAdvisor.Web.Perfiles;

/// <summary>El perfil activo se guarda en una cookie: "admin" o "alumno:ALU-001".</summary>
public static class PerfilActual
{
    public const string Cookie = "perfil";

    public static (string Tipo, string? AlumnoId) Leer(HttpRequest request)
    {
        var valor = request.Cookies[Cookie] ?? "";
        if (valor == "admin") return ("admin", null);
        if (valor.StartsWith("alumno:") && valor.Length > 7) return ("alumno", valor.Substring(7));
        return ("", null);
    }

    public static void Guardar(HttpResponse response, string valor) =>
        response.Cookies.Append(Cookie, valor, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

    public static void Borrar(HttpResponse response) => response.Cookies.Delete(Cookie);
}

/// <summary>Solo el administrador entra. El alumno va a su portal y quien no eligió perfil va al selector.</summary>
public class SoloAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var accion = context.RouteData.Values["action"]?.ToString() ?? "";
        if (accion == "Error" || accion == "Privacy") return;

        var (tipo, _) = PerfilActual.Leer(context.HttpContext.Request);
        if (tipo == "alumno") context.Result = new RedirectToActionResult("Index", "Portal", null);
        else if (tipo != "admin") context.Result = new RedirectToActionResult("Index", "Acceso", null);
    }
}

/// <summary>Solo el alumno entra a su portal.</summary>
public class SoloAlumnoAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var (tipo, _) = PerfilActual.Leer(context.HttpContext.Request);
        if (tipo == "admin") context.Result = new RedirectToActionResult("Index", "Home", null);
        else if (tipo != "alumno") context.Result = new RedirectToActionResult("Index", "Acceso", null);
    }
}

public record AlumnoResumen(string Id, string Nombre, int Semestre, double Promedio);
