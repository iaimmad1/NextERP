using System.Security.Claims;
using NextERP.Common.Constants;
using NextERP.Core.Interfaces;

namespace NextERP.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? UserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User?.FindFirst(CustomClaimTypes.UserId)?.Value;
                return int.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public int? TenantId
        {
            get
            {
                var tenantIdClaim = User?.FindFirst(CustomClaimTypes.TenantId)?.Value;
                return int.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
            }
        }

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Username => User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? FullName
        {
            get
            {
                var firstName = User?.FindFirst(CustomClaimTypes.FirstName)?.Value;
                var lastName = User?.FindFirst(CustomClaimTypes.LastName)?.Value;
                return string.IsNullOrEmpty(firstName) ? lastName : $"{firstName} {lastName}".Trim();
            }
        }

        public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value)
                                            ?? Enumerable.Empty<string>();

        public IEnumerable<string> Permissions => User?.FindAll(CustomClaimTypes.Permission).Select(c => c.Value)
                                                  ?? Enumerable.Empty<string>();

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public string? GetClaimValue(string claimType)
        {
            return User?.FindFirst(claimType)?.Value;
        }
    }
}
