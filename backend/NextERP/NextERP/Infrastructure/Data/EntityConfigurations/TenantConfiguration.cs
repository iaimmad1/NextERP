using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;
using NextERP.Core.Enums;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<TenantModel>
    {
        public void Configure(EntityTypeBuilder<TenantModel> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(t => t.TenantId);

            builder.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Subdomain)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.TaxId)
                .HasMaxLength(50);

            builder.Property(t => t.Address)
                .HasMaxLength(500);

            builder.Property(t => t.Phone)
                .HasMaxLength(20);

            builder.Property(t => t.Email)
                .HasMaxLength(100);

            builder.Property(t => t.LogoUrl)
                .HasMaxLength(500);

            builder.Property(t => t.Website)
                .HasMaxLength(200);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue(ENUM_UserStatus.Active);

            // Relationships
            builder.HasMany(t => t.Users)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Roles)
                .WithOne(r => r.Tenant)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(t => t.Subdomain)
                .IsUnique()
                .HasDatabaseName("IX_Tenants_Subdomain");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tenants_Status");
        }
    }
}
