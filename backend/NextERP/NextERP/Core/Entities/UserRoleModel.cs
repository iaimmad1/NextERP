using System.Data;
using Microsoft.EntityFrameworkCore;

namespace NextERP.Core.Entities
{
    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public virtual UserModel? User { get; set; }

        public int RoleId { get; set; }
        public virtual RoleModel? Role { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedBy { get; set; }
    }
}
