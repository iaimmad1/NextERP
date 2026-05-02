using NextERP.Core.Entities;

namespace NextERP.Core.Specifications
{
    /// <summary>
    /// Specification for Permission queries
    /// </summary>
    public class PermissionSpecifications
    {
        /// <summary>
        /// Get all active permissions
        /// </summary>
        public class GetActivePermissions : BaseSpecification<PermissionModel>
        {
            public GetActivePermissions()
            {
                Criteria = p => p.IsActive;
                ApplyOrderBy(p => p.Category);
            }
        }

        /// <summary>
        /// Get permissions by category
        /// </summary>
        public class GetPermissionsByCategory : BaseSpecification<PermissionModel>
        {
            public GetPermissionsByCategory(string category)
            {
                Criteria = p => p.Category == category && p.IsActive;
                ApplyOrderBy(p => p.Name);
            }
        }

        /// <summary>
        /// Get permissions assigned to a role
        /// </summary>
        public class GetRolePermissions : BaseSpecification<PermissionModel>
        {
            public GetRolePermissions(int roleId)
            {
                Criteria = p => p.RolePermissions.Any(rp => rp.RoleId == roleId);
                ApplyOrderBy(p => p.Category);
            }
        }

        /// <summary>
        /// Get permissions assigned to a user (through roles)
        /// </summary>
        public class GetUserPermissions : BaseSpecification<PermissionModel>
        {
            public GetUserPermissions(int userId)
            {
                Criteria = p => p.RolePermissions.Any(rp =>
                    rp.Role!.UserRoles.Any(ur => ur.UserId == userId));
                ApplyOrderBy(p => p.Category);
            }
        }
    }
}
