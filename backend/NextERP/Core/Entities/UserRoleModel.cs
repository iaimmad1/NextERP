using System;

namespace NextERP.Core.Entities
{
    public class UserRoleModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public UserModel? User { get; set; }
        public RoleModel? Role { get; set; }
    }
}
