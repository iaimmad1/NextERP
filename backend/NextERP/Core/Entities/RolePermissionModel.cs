using System;

namespace NextERP.Core.Entities
{
    public class RolePermissionModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public RoleModel? Role { get; set; }
        public PermissionModel? Permission { get; set; }
    }
}
