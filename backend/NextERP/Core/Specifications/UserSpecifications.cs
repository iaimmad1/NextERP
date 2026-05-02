using NextERP.Core.Entities;

namespace NextERP.Core.Specifications
{
    /// <summary>
    /// Specification for User queries
    /// Encapsulates different user-related query patterns
    /// </summary>
    public class UserSpecifications
    {
        /// <summary>
        /// Specification for getting active users by tenant with roles
        /// </summary>
        public class GetActiveUsersByTenant : BaseSpecification<UserModel>
        {
            public GetActiveUsersByTenant(int tenantId)
            {
                Criteria = u => u.TenantId == tenantId && u.IsActive;
                AddInclude(u => u.UserRoles);
                ApplyOrderBy(u => u.FirstName);
            }
        }

        /// <summary>
        /// Specification for getting user by ID with all details
        /// </summary>
        public class GetUserWithRolesAndPermissions : BaseSpecification<UserModel>
        {
            public GetUserWithRolesAndPermissions(int userId)
            {
                Criteria = u => u.Id == userId;
                AddInclude(u => u.UserRoles);
                AddInclude("UserRoles.Role");
                AddInclude("UserRoles.Role.RolePermissions");
                AddInclude("UserRoles.Role.RolePermissions.Permission");
            }
        }

        /// <summary>
        /// Specification for searching users by email in tenant
        /// </summary>
        public class SearchUsersByEmail : BaseSpecification<UserModel>
        {
            public SearchUsersByEmail(int tenantId, string emailSearch)
            {
                Criteria = u => u.TenantId == tenantId && u.Email.Contains(emailSearch);
                AddInclude(u => u.UserRoles);
                ApplyOrderBy(u => u.Email);
            }
        }

        /// <summary>
        /// Specification for getting users by role in tenant
        /// </summary>
        public class GetUsersByRole : BaseSpecification<UserModel>
        {
            public GetUsersByRole(int tenantId, int roleId)
            {
                Criteria = u => u.TenantId == tenantId && 
                               u.UserRoles.Any(ur => ur.RoleId == roleId);
                AddInclude(u => u.UserRoles);
            }
        }
    }
}
