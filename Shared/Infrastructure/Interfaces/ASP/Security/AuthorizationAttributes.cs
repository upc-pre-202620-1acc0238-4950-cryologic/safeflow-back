using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;

/// <summary>Requires a valid session (any role).</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAuthenticatedAttribute : Attribute, IAuthorizationFilter
{
    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;
        var user = context.HttpContext.FindCurrentUser();
        if (user is null) { context.Result = new UnauthorizedResult(); return; }
        if (!Allows(user))
            context.Result = new ObjectResult(new ErrorResource("forbidden", "No tienes permiso para realizar esta operación."))
                { StatusCode = StatusCodes.Status403Forbidden };
    }

    protected virtual bool Allows(CurrentUser user) => true;
}

/// <summary>Requires at least one of the given business permissions.</summary>
public sealed class RequirePermissionAttribute(params Permission[] permissions) : RequireAuthenticatedAttribute
{
    protected override bool Allows(CurrentUser user) => permissions.Any(user.Has);
}

/// <summary>Permission administration is inherent to ADMIN and non-delegable (FR-003).</summary>
public sealed class RequireAdminAttribute : RequireAuthenticatedAttribute
{
    protected override bool Allows(CurrentUser user) => user.IsAdmin;
}
