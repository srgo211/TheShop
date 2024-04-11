using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Claims;

namespace ExtensionsProject;

public class Extensions
{
    public static bool Check(HttpContext httpContext)
    {
        // Проверка авторизации
        if (!httpContext.User.Identity.IsAuthenticated || !httpContext.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "admin"))
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }
    }
}