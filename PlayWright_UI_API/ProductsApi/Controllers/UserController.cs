using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Repository;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetProfile()
    {
        // var username = User.Identity?.Name;
        var username = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized(new
            {
                message = "Username claim was not found in the token."
            });
        }

        var user = _userRepository.GetByUsername(username);

        if (user is null)
        {
            return NotFound(new
            {
                message = $"No user profile was found for '{username}'."
            });
        }

        return Ok(user);
    }
}
