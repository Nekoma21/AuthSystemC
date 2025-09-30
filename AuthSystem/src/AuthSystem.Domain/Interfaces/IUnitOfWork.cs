using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Permission> Permissions { get; }
    IRepository<UserRole> UserRoles { get; }
    IRepository<RolePermission> RolePermissions { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<TwoFactorCode> TwoFactorCodes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
