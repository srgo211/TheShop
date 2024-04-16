using Microsoft.AspNetCore.Http;
using SharedInterfaces;
using System.Text.Json;

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


    public static bool IsAdmin(string headerToken)
    {
        if(string.IsNullOrWhiteSpace(headerToken)) return false;
        if (headerToken.StartsWith("Bearer ")) headerToken = headerToken.Substring("Bearer ".Length).Trim();

        try
        {
            string json = JwtTokenService.ParseJwtTokenFromJson(headerToken);
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                string tupeRole = root.GetProperty("tupeRole").GetString();

                if (tupeRole.ToLower() == "admin") return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return false;




    }
}