using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) => _config = config;

    [HttpPost("dev-token")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public IActionResult DevToken([FromBody] DevTokenRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username))
            return BadRequest(new { error = "username required" });

        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = _config["Jwt:Issuer"] ?? "dev-issuer";
        var audience = _config["Jwt:Audience"] ?? "dev-audience";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, req.Username),
            new Claim(ClaimTypes.Name, req.Username)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims, expires: DateTime.UtcNow.AddHours(2), signingCredentials: creds);
        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expires = token.ValidTo });
    }
}

public class DevTokenRequest { public string Username { get; set; } = string.Empty; }