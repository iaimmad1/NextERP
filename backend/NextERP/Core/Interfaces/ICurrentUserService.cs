namespace NextERP.Core.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        int? TenantId { get; }
        string? Email { get; }
        IEnumerable<string> Roles { get; }
        IEnumerable<string> Permissions { get; }
    }
}
