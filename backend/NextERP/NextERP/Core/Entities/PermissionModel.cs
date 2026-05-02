namespace NextERP.Core.Entities
{
    public class PermissionModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public required string PermissionCategory { get; set; }
        public string? Module { get; set; }
        public bool IsSystemPermission { get; set; } = false;

        // Navigation properties
        public virtual ICollection<RolePermissionModel> RolePermissions { get; set; } = new List<RolePermissionModel>();
    }
}
