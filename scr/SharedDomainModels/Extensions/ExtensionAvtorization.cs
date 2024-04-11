using Microsoft.AspNetCore.Http;
using SharedInterfaces;
using System.Security.Claims;

namespace SharedDomainModels.Extensions;

public class ExtensionAvtorization
{
    /// <summary>Проверка авторизации </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static bool CheckAuthenticated(HttpContext httpContext)
    {
        if(httpContext is null) return false;

        string role = httpContext.User.Claims.FirstOrDefault(c => c.Type == "tupeRole")?.Value;
        
        if (httpContext.User.Identity.IsAuthenticated && 
            role == TupeRole.admin.ToString())
        {
            return true;
        }

        return false;
    }
}