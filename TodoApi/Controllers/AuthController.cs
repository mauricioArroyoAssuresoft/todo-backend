using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    public readonly TodoDbContext _context;
    private readonly PasswordHasher<User> _passwordHashser = new();

    public AuthController(TodoDbContext context)
    {
      _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
      if (await _context.Users.AnyAsync(user => user.Username == registerDto.Username))
      {
        return BadRequest(new { message = "username already exists" });
      }

      var user = new User { Username = registerDto.Username, Role = "Admin" };
      user.PasswordHash = _passwordHashser.HashPassword(user, registerDto.Password);
      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return Ok(new { message = "User registered securely" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == loginDto.Username);
      if (user is null)
      {
        return Unauthorized(new { message = "invalid credentials" });
      }

      var result = _passwordHashser.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
      if (result == PasswordVerificationResult.Failed)
      {
        return Unauthorized(new { message = "invalid credentials" });
      }


      var claims = new[]
      {
        new Claim(ClaimTypes.Name, loginDto.Username),
        new Claim(ClaimTypes.Role, "Admin")
      };
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyThatIsAtLeast32BytesLong"));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken(
        issuer: "TodoApi",
        audience: "TodoApiUsers",
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(60),
        signingCredentials: creds
      );


      return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
  }
}
