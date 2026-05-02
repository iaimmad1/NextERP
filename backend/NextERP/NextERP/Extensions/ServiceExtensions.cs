using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Common.Constants;
using NextERP.Core.Interfaces;
using NextERP.Infrastructure.Repositories;
using NextERP.Infrastructure.Security;
using NextERP.Infrastructure.Services;
using System.Reflection;

namespace NextERP.Extensions;

public static class ServiceExtensions
{
    // Main entry method called from Program.cs
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
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
            // Policy for each permission
            options.AddPolicy(Permissions.UsersView, policy => policy.RequireClaim("permission", Permissions.UsersView));
            options.AddPolicy(Permissions.UsersCreate, policy => policy.RequireClaim("permission", Permissions.UsersCreate));
            options.AddPolicy(Permissions.UsersEdit, policy => policy.RequireClaim("permission", Permissions.UsersEdit));
            options.AddPolicy(Permissions.UsersDelete, policy => policy.RequireClaim("permission", Permissions.UsersDelete));
            options.AddPolicy(Permissions.UsersManage, policy => policy.RequireClaim("permission", Permissions.UsersManage));

            options.AddPolicy(Permissions.RolesView, policy => policy.RequireClaim("permission", Permissions.RolesView));
            options.AddPolicy(Permissions.RolesCreate, policy => policy.RequireClaim("permission", Permissions.RolesCreate));
            options.AddPolicy(Permissions.RolesEdit, policy => policy.RequireClaim("permission", Permissions.RolesEdit));
            options.AddPolicy(Permissions.RolesDelete, policy => policy.RequireClaim("permission", Permissions.RolesDelete));
            options.AddPolicy(Permissions.RolesAssign, policy => policy.RequireClaim("permission", Permissions.RolesAssign));

            // Admin policy (requires any of the admin permissions)
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == "permission" &&
                        (c.Value == Permissions.UsersManage ||
                         c.Value == Permissions.RolesAssign ||
                         c.Value == Permissions.SystemSettings))));
        });
        return services;
    }

}