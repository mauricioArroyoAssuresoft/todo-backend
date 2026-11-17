using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoApi.DTOs;

namespace TodoApi.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
      if (loginDto.Username != "admin" || loginDto.Password != "12345")
      {
        return Unauthorized(new { message = "invalid credentials" });
      }

      var claims = new[]
      {
        new Claim(ClaimTypes.Name, loginDto.Username),
        new Claim(ClaimTypes.Role, "Admin")
      };
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyThatIsAtLeast32BytesLong!"));
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