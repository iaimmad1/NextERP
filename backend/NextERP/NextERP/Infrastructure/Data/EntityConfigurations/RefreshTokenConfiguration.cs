using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenModel>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenModel> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.RefreshTokeId);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.CreatedByIp)
                .HasMaxLength(45);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(45);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_Token");

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex(rt => rt.ExpiryDate)
                .HasDatabaseName("IX_RefreshTokens_ExpiryDate");

            builder.HasIndex(rt => new { rt.IsRevoked, rt.ExpiryDate })
                .HasDatabaseName("IX_RefreshTokens_IsRevoked_ExpiryDate");
        }
    }
}
