using System;
using System.Collections.Generic;

namespace NextERP.Core.Entities
{
    public class RoleModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public TenantModel? Tenant { get; set; }
        public ICollection<UserRoleModel> UserRoles { get; set; } = new HashSet<UserRoleModel>();
        public ICollection<RolePermissionModel> RolePermissions { get; set; } = new HashSet<RolePermissionModel>();
    }
}
