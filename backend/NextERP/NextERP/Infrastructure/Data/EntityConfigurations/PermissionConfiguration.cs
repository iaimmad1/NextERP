using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;
using NextERP.Core.Enums;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<PermissionModel>
    {
        public void Configure(EntityTypeBuilder<PermissionModel> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.PermissionId);

            builder.Property(p => p.PermissionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(200);

            builder.Property(p => p.PermissionCategory)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue(ENUM_PermissionCategory.SystemSettings);

            builder.Property(p => p.Module)
                .HasMaxLength(50);

            // Relationships
            builder.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.PermissionName)
                .IsUnique()
                .HasDatabaseName("IX_Permissions_Name");

            builder.HasIndex(p => p.PermissionCategory)
                .HasDatabaseName("IX_Permissions_Category");
        }
    }
}
