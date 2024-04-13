using IdentityAPI.Interfaces.Repositorys;
using Microsoft.AspNetCore.Mvc;
using SharedDomainModels;
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
    public async Task<ActionResult<User>> CreateUser(User user)
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

    [HttpGet("getUserFromGuid/{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        User? user = await userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }


    [HttpGet("getUserFromUserId/{id}")]
    public async Task<ActionResult<User>> GetUser(long id)
    {
        User? user = await userRepository.GetByUserIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }


    [HttpPut("updateUser/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, User updatedUser)
    {
        if (id != updatedUser.Guid)
        {
            return BadRequest("Mismatched user ID");
        }

        try
        {
            updatedUser.UpdatedAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(updatedUser);
            return NoContent();
        }
        catch (Exception)
        {
            if (!await UserExistsAsync(id))
            {
                return NotFound();
            }
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (!await UserExistsAsync(id))
        {
            return NotFound();
        }

        // Получение токена из заголовка Authorization
        var tokenHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (tokenHeader is null)
        {
            return Unauthorized("JWT token is missing or invalid.");
        }


        if (tokenHeader.StartsWith("Bearer ")) tokenHeader = tokenHeader.Substring("Bearer ".Length).Trim();

        var role = jwtTokenService.ParseJwtToken(tokenHeader)?.TupeRole ?? TupeRole.none;
        if (role != TupeRole.admin) return Unauthorized("Не явл администратором");

        await userRepository.DeleteAsync(id);
        return NoContent();
    }



    [HttpGet("isCheckUser/{id}")]
    public async Task<ActionResult<User>> CheckUser(Guid id)
    {
        bool check = await UserExistsAsync(id);
        return Ok(new { IsUser = check });

    }

    [HttpGet("isCheckUserFromUserId/{id}")]
    public async Task<ActionResult<User>> CheckUser(long id)
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