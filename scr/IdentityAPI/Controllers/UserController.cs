using IdentityAPI.Interfaces.Repositorys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharedDomainModels;
using SharedDomainModels.Extensions;
using SharedInterfaces;

namespace IdentityAPI.Controllers;

[Route("user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IJwtTokenService jwtTokenService;

    public UserController(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        this.userRepository = userRepository;
        this.jwtTokenService = jwtTokenService;
    }

    [HttpPost("addUser")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        User newUser = await userRepository.CreateAsync(user);
        string tokenJwt = default;
        try
        {
            tokenJwt = jwtTokenService.GenerateJwtToken(newUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return CreatedAtAction(nameof(GetUser), new { id = newUser.Guid, tokenJwt = tokenJwt}, newUser);
    }

    [HttpGet("getUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return Ok(await userRepository.GetAllAsync());
    }

    [HttpGet("getUserFromGuid")]
    public async Task<ActionResult<User>> GetUser([FromQuery] Guid guid)
    {
        User? user = await userRepository.GetByIdAsync(guid);
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }
    
    [HttpGet("getUserFromUserId")]
    public async Task<ActionResult<User>> GetUser([FromQuery] long id)
    {
        User? user = await userRepository.GetByUserIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }
    
    [HttpPut("updateUser")]
    public async Task<IActionResult> UpdateUser([FromQuery] Guid guid, [FromBody] User updatedUser)
    {

        
        var tokenHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        bool isAdmin = ExtensionAvtorization.IsAdmin(tokenHeader);


        User? user = await userRepository.GetByIdAsync(guid);
        if (user is null) return NotFound();


        try
        {
            updatedUser.Guid      = user.Guid;
            updatedUser.UserId    = user.UserId;
            updatedUser.CreatedAt = user.CreatedAt;

            if(!isAdmin) updatedUser.TupeRole = user.TupeRole;


            updatedUser.UpdatedAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(updatedUser);
            return NoContent();
        }
        catch (Exception)
        {
            if (!await UserExistsAsync(guid))
            {
                return NotFound();
            }
            throw;
        }
    }

    [HttpDelete("deleteUser")]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid guid)
    {
        if (!await UserExistsAsync(guid))
        {
            return NotFound();
        }

        // Получение токена из заголовка Authorization
        var tokenHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (tokenHeader is null)
        {
            return Unauthorized("JWT token is missing or invalid.");
        }

        bool isAdmin = ExtensionAvtorization.IsAdmin(tokenHeader);
        if (!isAdmin) return Unauthorized("Не явл администратором");

        await userRepository.DeleteAsync(guid);
        return NoContent();
    }

    [HttpGet("isCheckUser")]
    public async Task<ActionResult<User>> CheckUser([FromQuery] Guid guid)
    {
        bool check = await UserExistsAsync(guid);
        return Ok(new { IsUser = check });
    }

    [HttpGet("isCheckUserFromUserId")]
    public async Task<ActionResult<User>> CheckUser([FromQuery] long id)
    {
        User? user = await userRepository.GetByUserIdAsync(id);
        bool check = user is not null;
        return Ok(new { IsUser = check });

    }

    private async Task<bool> UserExistsAsync(Guid id)
    {
        User? user = await userRepository.GetByIdAsync(id);
        return user is not null;
    }
}