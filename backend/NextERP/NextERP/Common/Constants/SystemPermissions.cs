using NextERP.Core.Entities;
using NextERP.Core.Enums;

namespace NextERP.Common.Constants
{
    public static class SystemPermissions
    {
        public static List<PermissionModel> GetAllPermissions()
        {
            return new List<PermissionModel>
        {
            // User Management
            new PermissionModel
            {
                PermissionName = Permissions.UsersView,
                DisplayName = "View Users",
                PermissionCategory = ENUM_PermissionCategory.UserManagement,
                Description = "Can view the list of users and user details"
            },
            new PermissionModel
            {
                PermissionName = Permissions.UsersCreate,
                DisplayName = "Create Users",
                PermissionCategory = ENUM_PermissionCategory.UserManagement,
                Description = "Can create new users"
            },
            new PermissionModel
            {
                PermissionName = Permissions.UsersEdit,
                DisplayName = "Edit Users",
                PermissionCategory = ENUM_PermissionCategory.UserManagement,
                Description = "Can edit existing user information"
            },
            new PermissionModel
            {
                PermissionName = Permissions.UsersDelete,
                DisplayName = "Delete Users",
                PermissionCategory = ENUM_PermissionCategory.UserManagement,
                Description = "Can delete users from the system"
            },
            new PermissionModel
            {
                PermissionName = Permissions.UsersManage,
                DisplayName = "Manage Users",
                PermissionCategory = ENUM_PermissionCategory.UserManagement,
                Description = "Full management of users (including status changes)"
            },
            
            // Role Management
            new PermissionModel
            {
                PermissionName = Permissions.RolesView,
                DisplayName = "View Roles",
                PermissionCategory = ENUM_PermissionCategory.RoleManagement,
                Description = "Can view roles and their permissions"
            },
            new PermissionModel
            {
                PermissionName = Permissions.RolesCreate,
                DisplayName = "Create Roles",
                PermissionCategory = ENUM_PermissionCategory.RoleManagement,
                Description = "Can create new roles"
            },
            new PermissionModel
            {
                PermissionName = Permissions.RolesEdit,
                DisplayName = "Edit Roles",
                PermissionCategory = ENUM_PermissionCategory.RoleManagement,
                Description = "Can edit role names and descriptions"
            },
            new PermissionModel
            {
                PermissionName = Permissions.RolesDelete,
                DisplayName = "Delete Roles",
                PermissionCategory = ENUM_PermissionCategory.RoleManagement,
                Description = "Can delete roles (except system roles)"
            },
            new PermissionModel
            {
                PermissionName = Permissions.RolesAssign,
                DisplayName = "Assign Roles",
                PermissionCategory = ENUM_PermissionCategory.RoleManagement,
                Description = "Can assign or remove roles from users"
            },
            
            // Tenant Management (usually only for super admin)
            new PermissionModel
            {
                PermissionName = Permissions.TenantsView,
                DisplayName = "View Tenants",
                PermissionCategory = ENUM_PermissionCategory.TenantManagement,
                Description = "Can view all tenants"
            },
            new PermissionModel
            {
                PermissionName = Permissions.TenantsCreate,
                DisplayName = "Create Tenants",
                PermissionCategory = ENUM_PermissionCategory.TenantManagement,
                Description = "Can create new tenants"
            },
            new PermissionModel
            {
                PermissionName = Permissions.TenantsEdit,
                DisplayName = "Edit Tenants",
                PermissionCategory = ENUM_PermissionCategory.TenantManagement,
                Description = "Can edit tenant information"
            },
            new PermissionModel
            {
                PermissionName = Permissions.TenantsDelete,
                DisplayName = "Delete Tenants",
                PermissionCategory = ENUM_PermissionCategory.TenantManagement,
                Description = "Can delete tenants"
            },
            
            // System Settings & Audit
            new PermissionModel
            {
                PermissionName = Permissions.SystemSettings,
                DisplayName = "System Settings",
                PermissionCategory = ENUM_PermissionCategory.SystemSettings,
                Description = "Can view and modify system settings"
            },
            new PermissionModel
            {
                PermissionName = Permissions.SystemAudit,
                DisplayName = "View Audit Logs",
                PermissionCategory = ENUM_PermissionCategory.Audit,
                Description = "Can view audit logs and system activity"
            }
        };
        }
    }
}
