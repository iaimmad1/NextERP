using System;
using System.Collections.Generic;

namespace NextERP.Core.Entities
{
    public class PermissionModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<RolePermissionModel> RolePermissions { get; set; } = new HashSet<RolePermissionModel>();
    }
}
