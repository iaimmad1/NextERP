namespace NextERP.Common.Constants
{
    /// <summary>
    /// Centralized permission constants used across the application.
    /// Organized by feature/domain for maintainability.
    /// </summary>
    public static class Permissions
    {
        // User Management Permissions
        public static class Users
        {
            public const string Create = "users:create";
            public const string Read = "users:read";
            public const string Update = "users:update";
            public const string Delete = "users:delete";
            public const string Activate = "users:activate";
            public const string Deactivate = "users:deactivate";
            public const string AssignRoles = "users:assign_roles";
            public const string ViewAll = "users:view_all";
        }

        // Role Management Permissions
        public static class Roles
        {
            public const string Create = "roles:create";
            public const string Read = "roles:read";
            public const string Update = "roles:update";
            public const string Delete = "roles:delete";
            public const string AssignPermissions = "roles:assign_permissions";
            public const string ViewAll = "roles:view_all";
        }

        // Permission Management Permissions
        public static class Permissions_Management
        {
            public const string Read = "permissions:read";
            public const string Manage = "permissions:manage";
        }

        // Tenant Management Permissions
        public static class Tenants
        {
            public const string Create = "tenants:create";
            public const string Read = "tenants:read";
            public const string Update = "tenants:update";
            public const string Delete = "tenants:delete";
            public const string Activate = "tenants:activate";
            public const string Deactivate = "tenants:deactivate";
        }

        // Auth Permissions
        public static class Auth
        {
            public const string Login = "auth:login";
            public const string Register = "auth:register";
            public const string ChangePassword = "auth:change_password";
            public const string RefreshToken = "auth:refresh_token";
        }

        /// <summary>
        /// Get all permissions as a flat list.
        /// Useful for seeding database or validation.
        /// </summary>
        public static IEnumerable<string> GetAllPermissions()
        {
            return typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Static))
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null)!)
                .ToList();
        }
    }
}
