using NextERP.Features.Roles.DTOs;

namespace NextERP.Features.Permissions.DTOs
{
    public class PermissionCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
