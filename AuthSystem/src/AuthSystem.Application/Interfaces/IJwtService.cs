using AuthSystem.Domain.Entities;
using System.Security.Claims;

namespace AuthSystem.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, List<string> roles, List<string> permissions);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
}
