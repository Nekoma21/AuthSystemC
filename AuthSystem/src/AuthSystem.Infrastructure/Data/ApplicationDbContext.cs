using AuthSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<TwoFactorCode> TwoFactorCodes => Set<TwoFactorCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TwoFactorCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.TwoFactorCodes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "Administrator role with full access",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Standard user role",
                CreatedAt = DateTime.UtcNow
            }
        );

        var permissions = new[]
        {
            new Permission
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "User Read",
                Resource = "User",
                Action = "Read",
                Description = "Can read user data",
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "User Write",
                Resource = "User",
                Action = "Write",
                Description = "Can create and update users",
                CreatedAt = DateTime.UtcNow
            },
            new Permission
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "User Delete",
                Resource = "User",
                Action = "Delete",
                Description = "Can delete users",
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<Permission>().HasData(permissions);

        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                RoleId = adminRoleId,
                PermissionId = permissions[0].Id,
                CreatedAt = DateTime.UtcNow
            },
            new RolePermission
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                RoleId = adminRoleId,
                PermissionId = permissions[1].Id,
                CreatedAt = DateTime.UtcNow
            },
            new RolePermission
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                RoleId = adminRoleId,
                PermissionId = permissions[2].Id,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
