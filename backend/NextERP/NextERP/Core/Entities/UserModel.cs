namespace NextERP.Core.Entities
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }

        public int TenantId { get; set; }
        public virtual TenantModel? Tenant { get; set; }

        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }

        // Navigation properties
        public virtual ICollection<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
        public virtual ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new List<RefreshTokenModel>();
        public virtual ICollection<AuditLogModel> AuditLogs { get; set; } = new List<AuditLogModel>();

        // Computed properties
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();
    }
}
