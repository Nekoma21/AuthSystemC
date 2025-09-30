using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var firstName = User.FindFirst("firstName")?.Value;
        var lastName = User.FindFirst("lastName")?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        var permissions = User.FindAll("permission").Select(c => c.Value).ToList();

        return Ok(new
        {
            id = userId,
            email = email,
            firstName = firstName,
            lastName = lastName,
            roles = roles,
            permissions = permissions
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(new { message = "This endpoint returns all users - only accessible by Admins" });
    }
}
