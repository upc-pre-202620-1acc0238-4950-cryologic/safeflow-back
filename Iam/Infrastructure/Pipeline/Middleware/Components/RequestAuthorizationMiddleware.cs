using System.Net.Http.Headers;
using SafeFlow.API.Iam.Application.Outbound;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Iam.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenService tokens, IUserRepository users,
        IRevokedTokenRepository revokedTokens, IRolePermissionQueryService permissions)
    {
        if (HttpMethods.IsOptions(context.Request.Method)
            || context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            await next(context);
            return;
        }

        if (authorization.Count != 1
            || !AuthenticationHeaderValue.TryParse(authorization[0], out var header)
            || !string.Equals(header.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(header.Parameter))
        {
            await DenyAsync(context);
            return;
        }

        var session = tokens.Validate(header.Parameter);
        if (session is null || await revokedTokens.ContainsAsync(session.Jti, context.RequestAborted))
        {
            await DenyAsync(context);
            return;
        }

        var user = await users.FindByIdAsync(session.UserId, context.RequestAborted);
        if (user is null || user.Email != session.Email || user.Role != session.Role)
        {
            await DenyAsync(context);
            return;
        }

        var granted = await permissions.EffectivePermissionsAsync(user.Role, context.RequestAborted);
        context.SetCurrentUser(new CurrentUser(user.Id, user.Email, user.Role, granted));
        context.Items[typeof(ValidatedSession)] = session;
        await next(context);
    }

    private static async Task DenyAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.WWWAuthenticate = "Bearer";
        await context.Response.WriteAsJsonAsync(
            new ErrorResource("invalid_session", "Tu sesión terminó. Inicia sesión nuevamente."), context.RequestAborted);
    }
}
