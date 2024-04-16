using IdentityAPI.Interfaces.Repositorys;
using Microsoft.AspNetCore.Mvc;
using SharedInterfaces;

namespace IdentityAPI.Controllers;

[ApiController]
[Route("token")]
public class TokenController : ControllerBase
{
    private readonly IJwtTokenService jwtTokenService;
    private readonly IUserRepository userRepository;

    public TokenController(IJwtTokenService jwtTokenService, IUserRepository userRepository)
    {
        this.jwtTokenService = jwtTokenService;
        this.userRepository = userRepository;
    }

    [HttpGet("getToken")]
    public async Task<IActionResult> GenerateToken([FromQuery] Guid guid)
    {
        // Получение пользователя по GUID
        var user = await userRepository.GetByIdAsync(guid);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Генерация JWT токена
        var token = jwtTokenService.GenerateJwtToken(user);

        // Возвращаем токен в JSON формате
        return Ok(new { Token = token });
    }
}