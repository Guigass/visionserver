using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VisionAPI.Extensions;

public class CustomAuthorize
{
    public static bool ValidateClaimsUser(HttpContext context, string claimName, string claimValue)
    {
        return context.User.Identity!.IsAuthenticated && context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
    }

    public static bool ValidadeRoleUser(HttpContext context, string claimName)
    {
        return context.User.Identity!.IsAuthenticated && context.User.IsInRole(claimName);
    }
}

public class ClaimsAuthorizeAttribute : TypeFilterAttribute
{
    public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))
    {
        Arguments = new object[] { new Claim(claimName, claimValue) };
    }
}

public class RequisitoClaimFilter : IAuthorizationFilter
{
    private readonly Claim _claim;

    public RequisitoClaimFilter(Claim claim)
    {
        _claim = claim;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity!.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(401);
            return;
        }

        if (!CustomAuthorize.ValidateClaimsUser(context.HttpContext, _claim.Type, _claim.Value))
            context.Result = new ForbidResult();
    }
}

public class RoleAuthorizeAttribute : TypeFilterAttribute
{
    public RoleAuthorizeAttribute(string roleName) : base(typeof(RequisitoRoleFilter))
    {
        Arguments = new object[] { roleName };
    }
}

public class RequisitoRoleFilter : IAuthorizationFilter
{
    private readonly string _roleName;

    public RequisitoRoleFilter(string roleName)
    {
        _roleName = roleName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity!.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(401);
            return;
        }

        if (!CustomAuthorize.ValidadeRoleUser(context.HttpContext, _roleName))
            context.Result = new ForbidResult();
    }
}
