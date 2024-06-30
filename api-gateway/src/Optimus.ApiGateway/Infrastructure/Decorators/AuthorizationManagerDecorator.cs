using System.Security.Claims;
using Ntrada;

namespace Exchange.ApiGateway.Infrastructure.Decorators;

public class AuthorizationManagerDecorator : IAuthorizationManager
{
    private readonly IAuthorizationManager _authorizationManager;

    public AuthorizationManagerDecorator(IAuthorizationManager authorizationManager)
    {
        _authorizationManager = authorizationManager;
    }

    public bool IsAuthorized(ClaimsPrincipal user, RouteConfig routeConfig)
    {
        if (!routeConfig.Route.Claims.ContainsKey("role")) return _authorizationManager.IsAuthorized(user, routeConfig);
        
        var value = routeConfig.Route.Claims["role"];
        routeConfig.Route.Claims.Remove("role");
        routeConfig.Route.Claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] = value;

        return _authorizationManager.IsAuthorized(user, routeConfig);
    }
}