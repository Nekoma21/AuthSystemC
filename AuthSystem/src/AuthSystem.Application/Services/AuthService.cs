using AuthSystem.Application.DTOs.Auth;
using AuthSystem.Application.DTOs.Common;
using AuthSystem.Application.Interfaces;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Password != request.ConfirmPassword)
            return Result<AuthResponse>.Failure("Passwords do not match");

        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUser != null)
            return Result<AuthResponse>.Failure("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsEmailVerified = false,
            IsTwoFactorEnabled = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        var userRole = await _unitOfWork.Roles.FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);
        if (userRole != null)
        {
            await _unitOfWork.UserRoles.AddAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = userRole.Id,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName, cancellationToken);

        var roles = new List<string> { "User" };
        var permissions = new List<string>();

        var accessToken = _jwtService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return Result<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            RequiresTwoFactor = false,
            User = MapToUserDto(user, roles, permissions)
        });
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid email or password");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("Account is deactivated");

        if (user.IsTwoFactorEnabled)
        {
            var code = await GenerateTwoFactorCodeAsync(user.Id, cancellationToken);
            if (!code.IsSuccess)
                return Result<AuthResponse>.Failure("Failed to generate 2FA code");

            await _emailService.SendTwoFactorCodeAsync(user.Email, code.Data!, cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                RequiresTwoFactor = true
            });
        }

        return await GenerateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task<Result<AuthResponse>> VerifyTwoFactorAsync(TwoFactorRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null)
            return Result<AuthResponse>.Failure("User not found");

        var twoFactorCode = await _unitOfWork.TwoFactorCodes.FirstOrDefaultAsync(
            c => c.UserId == user.Id && c.Code == request.Code && c.IsValid,
            cancellationToken);

        if (twoFactorCode == null)
            return Result<AuthResponse>.Failure("Invalid or expired 2FA code");

        twoFactorCode.IsUsed = true;
        twoFactorCode.UsedAt = DateTime.UtcNow;
        await _unitOfWork.TwoFactorCodes.UpdateAsync(twoFactorCode, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GenerateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(
            t => t.Token == refreshToken && t.IsActive,
            cancellationToken);

        if (token == null)
            return Result<AuthResponse>.Failure("Invalid refresh token");

        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId, cancellationToken);
        if (user == null || !user.IsActive)
            return Result<AuthResponse>.Failure("User not found or inactive");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        await _unitOfWork.RefreshTokens.UpdateAsync(token, cancellationToken);

        return await GenerateAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task<Result<bool>> RevokeTokenAsync(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
    {
        var token = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(
            t => t.Token == refreshToken && t.IsActive,
            cancellationToken);

        if (token == null)
            return Result<bool>.Failure("Token not found");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        await _unitOfWork.RefreshTokens.UpdateAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> EnableTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result<bool>.Failure("User not found");

        user.IsTwoFactorEnabled = true;
        user.TwoFactorSecret = GenerateSecret();
        user.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DisableTwoFactorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result<bool>.Failure("User not found");

        user.IsTwoFactorEnabled = false;
        user.TwoFactorSecret = null;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> GenerateTwoFactorCodeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var code = new Random().Next(100000, 999999).ToString();

        var twoFactorCode = new TwoFactorCode
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Code = code,
            Type = TwoFactorCodeType.Email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TwoFactorCodes.AddAsync(twoFactorCode, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(code);
    }

    private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(User user, string ipAddress, CancellationToken cancellationToken)
    {
        var userRoles = await _unitOfWork.UserRoles.FindAsync(ur => ur.UserId == user.Id, cancellationToken);
        var roles = new List<string>();
        var permissions = new List<string>();

        foreach (var userRole in userRoles)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(userRole.RoleId, cancellationToken);
            if (role != null)
            {
                roles.Add(role.Name);
                var rolePermissions = await _unitOfWork.RolePermissions.FindAsync(rp => rp.RoleId == role.Id, cancellationToken);
                foreach (var rp in rolePermissions)
                {
                    var permission = await _unitOfWork.Permissions.GetByIdAsync(rp.PermissionId, cancellationToken);
                    if (permission != null)
                        permissions.Add($"{permission.Resource}:{permission.Action}");
                }
            }
        }

        var accessToken = _jwtService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);

        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            RequiresTwoFactor = false,
            User = MapToUserDto(user, roles, permissions)
        });
    }

    private UserDto MapToUserDto(User user, List<string> roles, List<string> permissions)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsEmailVerified = user.IsEmailVerified,
            IsTwoFactorEnabled = user.IsTwoFactorEnabled,
            Roles = roles,
            Permissions = permissions
        };
    }

    private string GenerateSecret()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
