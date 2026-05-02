namespace NextERP.Features.Roles.DTOs
{
    public class RoleResponseDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public string Status { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RoleDetailDto : RoleResponseDto
    {
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string PermissionCategory { get; set; } = string.Empty;
    }
}
