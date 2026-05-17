namespace NextERP.Common.Constants
{

    public static class Permissions
    {
        // User Management
        public const string UsersView = "users.view";
        public const string UsersCreate = "users.create";
        public const string UsersEdit = "users.edit";
        public const string UsersDelete = "users.delete";
        public const string UsersManage = "users.manage";

        // Role Management
        public const string RolesView = "roles.view";
        public const string RolesCreate = "roles.create";
        public const string RolesEdit = "roles.edit";
        public const string RolesDelete = "roles.delete";
        public const string RolesAssign = "roles.assign";

        // Tenant Management
        public const string TenantsView = "tenants.view";
        public const string TenantsCreate = "tenants.create";
        public const string TenantsEdit = "tenants.edit";
        public const string TenantsDelete = "tenants.delete";

        // System
        public const string SystemSettings = "system.settings";
        public const string SystemAudit = "system.audit";

        // Product Management
        public const string ProductsView = "products.view";
        public const string ProductsCreate = "products.create";
        public const string ProductsEdit = "products.edit";
        public const string ProductsDelete = "products.delete";

        // Order Management
        public const string OrdersView = "orders.view";
        public const string OrdersCreate = "orders.create";
        public const string OrdersUpdate = "orders.update";
    }
}