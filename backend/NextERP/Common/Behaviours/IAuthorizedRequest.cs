namespace NextERP.Common.Behaviours
{
    /// <summary>
    /// Marker interface for requests that require authorization.
    /// Implement this on Commands/Queries that need permission checks.
    /// Example: public class CreateUserCommand : IRequest<ApiResponse<UserResponseDto>>, IAuthorizedRequest
    /// </summary>
    public interface IAuthorizedRequest
    {
        /// <summary>
        /// List of required permissions to execute this request.
        /// </summary>
        IEnumerable<string> RequiredPermissions { get; }
    }
}
