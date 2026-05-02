using System;
using System.Collections.Generic;

namespace NextERP.Core.Entities
{
    public class UserModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public TenantModel? Tenant { get; set; }
        public ICollection<UserRoleModel> UserRoles { get; set; } = new HashSet<UserRoleModel>();
        public ICollection<RefreshTokenModel> RefreshTokens { get; set; } = new HashSet<RefreshTokenModel>();
    }
}
