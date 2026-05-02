namespace NextERP.Common.Constants
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly List<string> AllRoles = new()
    {
        SuperAdmin,
        Admin,
        User
    };

        public static bool IsValidRole(string role) => AllRoles.Contains(role);
    }
}
