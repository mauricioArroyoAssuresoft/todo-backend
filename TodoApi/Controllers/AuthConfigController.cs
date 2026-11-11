using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TodoApi.Configuration;

namespace TodoApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthConfigController : ControllerBase
  {
    private readonly JwtSettings _jwtSettings;

    public AuthConfigController(IOptions<JwtSettings> jwtSettings)
    {
      _jwtSettings = jwtSettings.Value;
    }

    [HttpGet]
    public IActionResult GetIssuer()
    {
      return Ok(new { Issuer = _jwtSettings.Issuer, ExpiryMinutes = _jwtSettings.ExpiryInMinutes });
    }
  }
}