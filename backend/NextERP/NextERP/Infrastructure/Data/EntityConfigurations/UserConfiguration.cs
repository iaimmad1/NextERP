using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;
using NextERP.Core.Enums;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.MiddleName)
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.Gender)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue(ENUM_Gender.PreferNotToSay);

            builder.Property(u => u.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue(ENUM_UserStatus.Active);

            builder.Property(u => u.ProfilePictureUrl)
                .HasMaxLength(500);

            builder.Property(u => u.LastLoginIp)
                .HasMaxLength(45);

            // Relationships
            builder.HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(u => new { u.Username, u.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Users_Username_TenantId");

            builder.HasIndex(u => new { u.Email, u.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Users_Email_TenantId");

            builder.HasIndex(u => u.Status)
                .HasDatabaseName("IX_Users_Status");

            builder.HasIndex(u => u.TenantId)
                .HasDatabaseName("IX_Users_TenantId");
        }
    }
}
