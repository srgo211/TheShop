namespace SharedInterfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(IUser user);
    IUser ParseJwtToken(string token);

    string ParseJwtTokenFromJson(string token);
}