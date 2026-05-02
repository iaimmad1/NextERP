using NextERP.Core.Enums;

namespace NextERP.Core.Entities
{
    public class AuditLogModel
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }
        public virtual UserModel? User { get; set; }
        public int? TenantId { get; set; }
        public virtual TenantModel? Tenant { get; set; }
        public required string Action { get; set; } 
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
