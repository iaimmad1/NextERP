using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;
using NextERP.Core.Enums;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<RoleModel>
    {
        public void Configure(EntityTypeBuilder<RoleModel> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue(ENUM_UserStatus.Active);

            // Relationships

            builder.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => new { r.RoleName, r.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Roles_RoleName_TenantId");

            builder.HasIndex(r => r.TenantId)
                .HasDatabaseName("IX_Roles_TenantId");

            builder.HasIndex(r => r.IsSystemRole)
                .HasDatabaseName("IX_Roles_IsSystemRole");
        }
    }
}
