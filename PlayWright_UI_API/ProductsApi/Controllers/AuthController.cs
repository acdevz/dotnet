using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Models;
using ProductsApi.Models;
using ProductsApi.Repository;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsApi.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private UserRepository _userRepository;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AuthController(IConfiguration config, UserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _config = config;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = _userRepository.GetByUsername(req.Username);
        if (user == null)
            return Unauthorized(new { message = "User not found. Create an account first." });

        var res = _passwordHasher.VerifyHashedPassword(user, user.Password, req.Password);
        if (res == PasswordVerificationResult.Success)
        {
            var token = GenerateToken(
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, req.Username)
            );
            return Ok(new { token });
        }
        else if (res == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.Password = _passwordHasher.HashPassword(user, req.Password);
            _userRepository.Update(user.Id, user);
        }
        return Unauthorized(new { message = "Invalid Credentials" });
    }

    [SwaggerIgnore]
    public string GenerateToken(params Claim[] claims)
    {
        /* *************************************** JWT ***************************************
             * JwtSecurityTokenHandler.CreateToken(SecurityTokenDescriptor) -> SecurityToken
             *                        .WriteToken(SecurityToken) -> string
             ************************************************************************************* */

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresTimeoutInMinutes"])),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
