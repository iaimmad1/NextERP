using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NextERP.Core.Entities;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Data.EntityConfigurations;

namespace NextERP.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<TenantModel> Tenants { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        public DbSet<RolePermissionModel> RolePermissions { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
        public DbSet<AuditLogModel> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new TenantConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Add audit logging for entities
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLogModel &&
                           (e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType().Name;
                var keyProperty = entry.Metadata.FindPrimaryKey()?.Properties.First();
                var entityId = keyProperty != null ? entry.Property(keyProperty.Name)?.CurrentValue?.ToString() : "unknown";

                // Set CreatedBy and UpdatedBy for auditable entities
                if (entry.Entity is UserModel user)
                {
                    if (entry.State == EntityState.Added)
                    {
                        user.CreatedBy = _currentUserService.UserId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        user.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.Entity is RoleModel role)
                {
                    if (entry.State == EntityState.Added)
                    {
                        role.CreatedBy = _currentUserService.UserId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        role.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.Entity is TenantModel tenant)
                {
                    if (entry.State == EntityState.Added)
                    {
                        tenant.CreatedBy = _currentUserService.UserId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        tenant.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

}
