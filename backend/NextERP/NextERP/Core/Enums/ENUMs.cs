namespace NextERP.Core.Enums
{
    public static class ENUM_Gender
    {
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Other = "Other";
        public const string PreferNotToSay = "PreferNotToSay";
    }
    public static class ENUM_UserStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Suspended = "Suspended";
        public const string PendingVerification = "PendingVerification";

    }
    public static class ENUM_PermissionCategory
    {
        public const string UserManagement = "UserManagement";
        public const string RoleManagement = "RoleManagement";
        public const string TenantManagement = "TenantManagement";
        public const string SystemSettings = "SystemSettings";
        public const string Reporting = "Reporting";
        public const string Audit = "Audit";
    }
    public static class ENUM_AuditAction
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string PermissionChange = "PermissionChange";
        public const string RoleAssignment = "RoleAssignment";
    }
}