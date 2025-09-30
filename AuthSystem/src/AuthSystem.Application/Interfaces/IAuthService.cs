using AuthSystem.Application.DTOs.Auth;
using AuthSystem.Application.DTOs.Common;

namespace AuthSystem.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> VerifyTwoFactorAsync(TwoFactorRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result<bool>> EnableTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DisableTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateTwoFactorCodeAsync(Guid userId, CancellationToken cancellationToken = default);
}
