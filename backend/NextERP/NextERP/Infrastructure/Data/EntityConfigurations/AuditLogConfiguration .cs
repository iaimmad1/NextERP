using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NextERP.Core.Entities;

namespace NextERP.Infrastructure.Data.EntityConfigurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogModel>
    {
        public void Configure(EntityTypeBuilder<AuditLogModel> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(al => al.AuditLogId);

            builder.Property(al => al.Action)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(al => al.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.EntityId)
                .HasMaxLength(100);

            builder.Property(al => al.IpAddress)
                .HasMaxLength(45);

            builder.Property(al => al.UserAgent)
                .HasMaxLength(500);

            builder.Property(al => al.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(al => al.NewValues)
                .HasColumnType("nvarchar(max)");

            // Relationships
            builder.HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(al => al.Tenant)
                .WithMany(t => t.AuditLogs)
                .HasForeignKey(al => al.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(al => al.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            builder.HasIndex(al => al.TenantId)
                .HasDatabaseName("IX_AuditLogs_TenantId");

            builder.HasIndex(al => al.EntityType)
                .HasDatabaseName("IX_AuditLogs_EntityType");

            builder.HasIndex(al => al.Action)
                .HasDatabaseName("IX_AuditLogs_Action");

            builder.HasIndex(al => al.CreatedAt)
                .HasDatabaseName("IX_AuditLogs_CreatedAt");
        }
    }
}
