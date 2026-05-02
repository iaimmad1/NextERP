using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogModel>
    {
        public void Configure(EntityTypeBuilder<AuditLogModel> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(a => new { a.TenantId, a.CreatedAt })
                .HasDatabaseName("IX_AuditLogs_TenantId_CreatedAt");
        }
    }
}
