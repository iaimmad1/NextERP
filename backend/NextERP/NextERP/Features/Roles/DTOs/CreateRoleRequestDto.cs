namespace NextERP.Features.Roles.DTOs
{
    public class CreateRoleRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
