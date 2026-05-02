namespace NextERP.Common.Constants
{
    public static class ErrorMessages
    {
        // Authentication
        public const string InvalidCredentials = "Invalid username or password";
        public const string AccountLocked = "Account is locked. Please contact support";
        public const string AccountNotActive = "Account is not active";
        public const string InvalidToken = "Invalid or expired token";
        public const string RefreshTokenNotFound = "Refresh token not found";
        public const string RefreshTokenExpired = "Refresh token has expired";

        // User
        public const string UserNotFound = "User not found";
        public const string UserAlreadyExists = "User already exists";
        public const string EmailAlreadyExists = "Email already exists";
        public const string UsernameAlreadyExists = "Username already exists";

        // Role
        public const string RoleNotFound = "Role not found";
        public const string RoleAlreadyExists = "Role already exists";
        public const string CannotDeleteSystemRole = "Cannot delete system role";

        // Tenant
        public const string TenantNotFound = "Tenant not found";
        public const string SubdomainAlreadyExists = "Subdomain already exists";
        public const string TenantInactive = "Tenant is inactive";

        // Permission
        public const string InsufficientPermissions = "Insufficient permissions";
        public const string PermissionNotFound = "Permission not found";

        // General
        public const string SomethingWentWrong = "Something went wrong. Please try again later";
        public const string InvalidRequest = "Invalid request";
        public const string Unauthorized = "Unauthorized access";
    }
}
