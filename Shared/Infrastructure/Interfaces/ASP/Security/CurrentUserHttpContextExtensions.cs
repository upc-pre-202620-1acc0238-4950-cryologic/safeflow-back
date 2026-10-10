using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;

public static class CurrentUserHttpContextExtensions
{
    public const string Key = "CurrentUser";

    public static CurrentUser? FindCurrentUser(this HttpContext context) =>
        context.Items.TryGetValue(Key, out var value) ? value as CurrentUser : null;

    /// <summary>Use only behind [RequireAuthenticated], [RequirePermission] or [RequireAdmin].</summary>
    public static CurrentUser GetCurrentUser(this HttpContext context) =>
        context.FindCurrentUser() ?? throw new InvalidOperationException("No authenticated user.");

    public static void SetCurrentUser(this HttpContext context, CurrentUser user) => context.Items[Key] = user;
}
