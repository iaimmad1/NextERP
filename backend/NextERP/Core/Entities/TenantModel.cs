using System;
using System.Collections.Generic;

namespace NextERP.Core.Entities
{
    public class TenantModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<UserModel> Users { get; set; } = new HashSet<UserModel>();
        public ICollection<RoleModel> Roles { get; set; } = new HashSet<RoleModel>();
    }
}
