using System.Data;
using System.Security;

namespace NextERP.Core.Entities
{
    public class RolePermissionModel
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public virtual RoleModel Role { get; set; }

        public int PermissionId { get; set; }
        public virtual PermissionModel Permission { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedBy { get; set; }
    }
}
