using Microsoft.IdentityModel.Tokens;
using SharedInterfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SharedDomainModels;

public class JwtTokenService : IJwtTokenService
{
    private readonly string secretKeyHere;
    private readonly string validIssuerAndAudience;

    public JwtTokenService(string secretKeyHere, string validIssuerAndAudience)
    {
        this.secretKeyHere = secretKeyHere; 
        this.validIssuerAndAudience = validIssuerAndAudience;   
    }

    public string GenerateJwtToken(IUser user)
    {
        if (user is null) return default;

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyHere));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);



        var claims = new[]
        {
            new Claim("guid", user.Guid.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString() ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("userName", user.UserName ?? ""),
            new Claim("tupeRole", user.TupeRole.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),


        };

        var tokenOptions = new JwtSecurityToken(
            issuer: validIssuerAndAudience,
            audience: validIssuerAndAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signinCredentials
        );

        string token =  new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return token;
    }

    public IUser ParseJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return default;
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyHere)),
            ValidateIssuer = true,
            ValidIssuer = validIssuerAndAudience,
            ValidateAudience = true,
            ValidAudience = validIssuerAndAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        SecurityToken validatedToken;
        ClaimsPrincipal principal = default;
        try
        {
            principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        var jwtToken = (JwtSecurityToken)validatedToken;

        //foreach (var claim in principal.Claims) Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");


        return new User
        {
            UserId = long.Parse(principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0"),
            UserName = principal.Claims.FirstOrDefault(c => c.Type == "userName")?.Value,
            Email = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
            TupeRole = Enum.Parse<TupeRole>(principal.Claims.FirstOrDefault(c => c.Type == "tupeRole")?.Value ?? "none"),
            Guid = Guid.Parse(principal.Claims.FirstOrDefault(c => c.Type == "guid")?.Value ?? Guid.Empty.ToString()),
        };
    }

    public static string ParseJwtTokenFromJson(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(jwtToken.Payload, 
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        return payloadJson;
    }

}