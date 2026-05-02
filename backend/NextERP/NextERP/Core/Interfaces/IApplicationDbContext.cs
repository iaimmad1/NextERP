using Microsoft.EntityFrameworkCore;
using NextERP.Core.Entities;

namespace NextERP.Core.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<TenantModel> Tenants { get; set; }
        DbSet<UserModel> Users { get; set; }
        DbSet<RoleModel> Roles { get; set; }
        DbSet<PermissionModel> Permissions { get; set; }
        DbSet<UserRoleModel> UserRoles { get; set; }
        DbSet<RolePermissionModel> RolePermissions { get; set; }
        DbSet<RefreshTokenModel> RefreshTokens { get; set; }
        DbSet<AuditLogModel> AuditLogs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
