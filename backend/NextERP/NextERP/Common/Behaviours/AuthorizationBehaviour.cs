using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NextERP.Common.DTOs;
using NextERP.Core.Interfaces;

namespace NextERP.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehaviour(
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IAuthorizedRequest authorizedRequest)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                // Assuming TResponse is ApiResponse<T>
                // For now, return unauthorized
                throw new UnauthorizedAccessException("No HTTP context available");
            }

            // Check permission if specified
            if (!string.IsNullOrEmpty(authorizedRequest.RequiredPermission))
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(
                    httpContext.User,
                    null,
                    authorizedRequest.RequiredPermission);

                if (!authorizationResult.Succeeded)
                {
                    throw new UnauthorizedAccessException($"Permission '{authorizedRequest.RequiredPermission}' is required");
                }
            }

            // Check role if specified
            if (!string.IsNullOrEmpty(authorizedRequest.RequiredRole))
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(
                    httpContext.User,
                    null,
                    authorizedRequest.RequiredRole);

                if (!authorizationResult.Succeeded)
                {
                    throw new UnauthorizedAccessException($"Role '{authorizedRequest.RequiredRole}' is required");
                }
            }
        }

        return await next();
    }
}