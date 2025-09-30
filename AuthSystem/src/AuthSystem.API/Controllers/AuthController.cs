using AuthSystem.Application.DTOs.Auth;
using AuthSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _authService.LoginAsync(request, ipAddress, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFactorRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _authService.VerifyTwoFactorAsync(request, ipAddress, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(new { message = "Token revoked successfully" });
    }

    [Authorize]
    [HttpPost("enable-2fa")]
    public async Task<IActionResult> EnableTwoFactor(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _authService.EnableTwoFactorAsync(userId, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(new { message = "Two-factor authentication enabled successfully" });
    }

    [Authorize]
    [HttpPost("disable-2fa")]
    public async Task<IActionResult> DisableTwoFactor(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _authService.DisableTwoFactorAsync(userId, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Ok(new { message = "Two-factor authentication disabled successfully" });
    }
}
