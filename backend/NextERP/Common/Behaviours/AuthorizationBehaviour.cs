using MediatR;
using NextERP.Core.Interfaces;

namespace NextERP.Common.Behaviours
{
    /// <summary>
    /// MediatR Pipeline Behaviour for authorization checks.
    /// Executes before the actual handler to validate permissions.
    /// </summary>
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IAuthorizedRequest
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehaviour(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requiredPermissions = request.RequiredPermissions?.ToList() ?? new List<string>();

            // If no specific permissions required, proceed
            if (!requiredPermissions.Any())
            {
                return await next();
            }

            // Check if user is authenticated
            if (_currentUserService.UserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // Check if user has at least one of the required permissions
            var userPermissions = _currentUserService.Permissions?.ToList() ?? new List<string>();
            var hasPermission = requiredPermissions.Any(rp => userPermissions.Contains(rp));

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    $"User does not have required permissions. Required: {string.Join(", ", requiredPermissions)}");
            }

            return await next();
        }
    }
}
