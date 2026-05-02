using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using MediatR;
using NextERP.Common.Behaviours;
using NextERP.Core.Interfaces;
using NextERP.Core.Specifications;
using NextERP.Infrastructure.Data;
using NextERP.Infrastructure.Security;
using NextERP.Infrastructure.Services;
using NextERP.Common.Constants;

namespace NextERP.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            // Register FluentValidation
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            // Register pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtService, JwtService>();

            // Configure JWT settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Configure JWT authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

            // Register authorization policies
            services.AddAuthorization(options =>
            {
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

                options.AddPolicy(Permissions.TenantsView, policy => policy.RequireClaim("permission", Permissions.TenantsView));
                options.AddPolicy(Permissions.TenantsCreate, policy => policy.RequireClaim("permission", Permissions.TenantsCreate));
                options.AddPolicy(Permissions.TenantsEdit, policy => policy.RequireClaim("permission", Permissions.TenantsEdit));
                options.AddPolicy(Permissions.TenantsDelete, policy => policy.RequireClaim("permission", Permissions.TenantsDelete));

                options.AddPolicy(Permissions.SystemSettings, policy => policy.RequireClaim("permission", Permissions.SystemSettings));
                options.AddPolicy(Permissions.SystemAudit, policy => policy.RequireClaim("permission", Permissions.SystemAudit));
            });

            return services;
        }
    }
}
