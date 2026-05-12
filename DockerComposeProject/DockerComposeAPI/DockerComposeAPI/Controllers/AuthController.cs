using DockerComposeAPI.Data;
using DockerComposeAPI.DTO;
using DockerComposeAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DockerComposeAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(AppDbContext dbContext) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register([FromBody] UserDTO dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return  BadRequest("Passwords don't match");
        }

        var user = new User
        {
            UserName = dto.UserName,
            Password = dto.Password,
            Email = dto.Email
        };
        dbContext.Add(user);
        await dbContext.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = dbContext.Users.FirstOrDefault(x=>x.Email==dto.Email && x.Password==dto.Password);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        return Ok();
    }
}