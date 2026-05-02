using System.Security.Claims;

namespace NextERP.Core.Interfaces
{
    public interface ICurrentUserService
    {
        /// <summary>Current authenticated user ID</summary>
        int? UserId { get; }

        /// <summary>Current user's tenant ID</summary>
        int? TenantId { get; }

        /// <summary>Current user's email</summary>
        string? Email { get; }

        /// <summary>Current user's username</summary>
        string? Username { get; }

        /// <summary>Current user's full name</summary>
        string? FullName { get; }

        /// <summary>List of roles assigned to the current user</summary>
        IEnumerable<string> Roles { get; }

        /// <summary>List of permissions assigned to the current user (via roles)</summary>
        IEnumerable<string> Permissions { get; }

        /// <summary>Indicates if the user is authenticated</summary>
        bool IsAuthenticated { get; }

        /// <summary>Raw ClaimsPrincipal</summary>
        ClaimsPrincipal? User { get; }

        /// <summary>Check if the user has a specific role</summary>
        bool HasRole(string role);

        /// <summary>Check if the user has a specific permission</summary>
        bool HasPermission(string permission);

        /// <summary>Get a specific claim value</summary>
        string? GetClaimValue(string claimType);
    }
}
