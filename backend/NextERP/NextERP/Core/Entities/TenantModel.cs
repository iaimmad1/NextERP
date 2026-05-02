using System.Data;

namespace NextERP.Core.Entities
{
    public class TenantModel
    {
        public int TenantId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<UserModel> Users { get; set; } = new List<UserModel>();
        public virtual ICollection<RoleModel> Roles { get; set; } = new List<RoleModel>();
        public virtual ICollection<AuditLogModel> AuditLogs { get; set; } = new List<AuditLogModel>();
    }
}
