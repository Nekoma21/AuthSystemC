using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Infrastructure.Data;

namespace AuthSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IRepository<User>? _users;
    private IRepository<Role>? _roles;
    private IRepository<Permission>? _permissions;
    private IRepository<UserRole>? _userRoles;
    private IRepository<RolePermission>? _rolePermissions;
    private IRepository<RefreshToken>? _refreshTokens;
    private IRepository<TwoFactorCode>? _twoFactorCodes;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Role> Roles => _roles ??= new Repository<Role>(_context);
    public IRepository<Permission> Permissions => _permissions ??= new Repository<Permission>(_context);
    public IRepository<UserRole> UserRoles => _userRoles ??= new Repository<UserRole>(_context);
    public IRepository<RolePermission> RolePermissions => _rolePermissions ??= new Repository<RolePermission>(_context);
    public IRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new Repository<RefreshToken>(_context);
    public IRepository<TwoFactorCode> TwoFactorCodes => _twoFactorCodes ??= new Repository<TwoFactorCode>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
