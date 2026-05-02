namespace NextERP.Core.Entities
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public virtual TenantModel? Tenant { get; set; }
        public bool IsSystemRole { get; set; } = false;
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? Description { get; set; }  

        // Navigation properties
        public virtual ICollection<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
        public virtual ICollection<RolePermissionModel> RolePermissions { get; set; } = new List<RolePermissionModel>();
    }
}
