using FluentValidation;
using MediatR;
using NextERP.Common.Behaviours;
using System.Reflection;

namespace NextERP.Extensions
{
    /// <summary>
    /// Extension for registering MediatR with CQRS handlers and behaviours
    /// </summary>
    public static class MediatRExtensions
    {
        public static IServiceCollection AddMediatRCQRS(this IServiceCollection services)
        {
            // Register MediatR with assembly scanning for handlers
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                // Register pipeline behaviours in order
                cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            });

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
