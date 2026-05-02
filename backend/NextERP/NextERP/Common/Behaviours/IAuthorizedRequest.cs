namespace NextERP.Common.Behaviours
{
    public interface IAuthorizedRequest
    {
        string? RequiredPermission { get; }
        string? RequiredRole { get; }
    }
}
