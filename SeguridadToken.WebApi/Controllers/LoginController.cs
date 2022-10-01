using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SeguridadToken.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SeguridadToken.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IConfiguration _configuration;

    public LoginController(ILogger<LoginController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost(nameof(Login))]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var infoUsuario = await AutenticarUsuarioAsync(loginDto.User, loginDto.Password);
        if (infoUsuario != null)
            return Ok(new { token = GenerarTokenJWT(infoUsuario) });
        else
            return Unauthorized();
    }

    private async Task<UserDto?> AutenticarUsuarioAsync(string user, string password)
    {
        if ("admin" == user && "admin" == password)
        {
            return new UserDto
            {
                FirstName = "admin",
                LastName = "nimda",
                Email = "nimda.admin@user.com"
            };
        }

        return null;
    }

    private string GenerarTokenJWT(UserDto userDto)
    {
        var _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:ClaveSecreta"]));
        var _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var _Header = new JwtHeader(_signingCredentials);

        var _Claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim("nombre",userDto.FirstName),
            new Claim("apellido",userDto.LastName),
            new Claim(JwtRegisteredClaimNames.Email,userDto.Email),
        };

        var _Payload = new JwtPayload(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: _Claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(24)
            );

        var _Token = new JwtSecurityToken(_Header, _Payload);

        return new JwtSecurityTokenHandler().WriteToken(_Token);
    }
}