using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Repositories;
using NextERP.Infrastructure.Security;
using NextERP.Infrastructure.Services;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NextERP.Extensions;

public static class ServiceExtensions
{
    // Main entry method called from Program.cs
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Add Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var secret = configuration["JwtSettings:Secret"];
                var issuer = configuration["JwtSettings:Issuer"];
                var audience = configuration["JwtSettings:Audience"];

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "default_secret_key_that_is_at_least_32_chars_long"))
                };
            });

        services.AddAuthorizationPolicies();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Configure JWT settings from appsettings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

        // Add JWT services
        services.AddScoped<IJwtService, JwtService>();

        // ... other services
        return services;
    }
    private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Helper to add policy that allows if user has permission OR is an Admin
            void AddPermissionPolicy(string permission)
            {
                options.AddPolicy(permission, policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == "permission" && c.Value == permission) ||
                        context.User.IsInRole("Admin")));
            }

            // User Management
            AddPermissionPolicy(Permissions.UsersView);
            AddPermissionPolicy(Permissions.UsersCreate);
            AddPermissionPolicy(Permissions.UsersEdit);
            AddPermissionPolicy(Permissions.UsersDelete);
            AddPermissionPolicy(Permissions.UsersManage);

            // Role Management
            AddPermissionPolicy(Permissions.RolesView);
            AddPermissionPolicy(Permissions.RolesCreate);
            AddPermissionPolicy(Permissions.RolesEdit);
            AddPermissionPolicy(Permissions.RolesDelete);
            AddPermissionPolicy(Permissions.RolesAssign);

            // Product Management
            AddPermissionPolicy(Permissions.ProductsView);
            AddPermissionPolicy(Permissions.ProductsCreate);
            AddPermissionPolicy(Permissions.ProductsEdit);
            AddPermissionPolicy(Permissions.ProductsDelete);

            // Order Management
            AddPermissionPolicy(Permissions.OrdersView);
            AddPermissionPolicy(Permissions.OrdersCreate);
            AddPermissionPolicy(Permissions.OrdersUpdate);

            // Tenant Management
            AddPermissionPolicy(Permissions.TenantsView);
            AddPermissionPolicy(Permissions.TenantsEdit);

            // Admin policy (requires any of the admin permissions)
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") ||
                    context.User.HasClaim(c => c.Type == "permission" &&
                        (c.Value == Permissions.UsersManage ||
                         c.Value == Permissions.RolesAssign ||
                         c.Value == Permissions.SystemSettings))));
        });
        return services;
    }

}