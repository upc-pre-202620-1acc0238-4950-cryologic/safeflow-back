namespace SafeFlow.API.Shared.Domain.Model.Security;

/// <summary>Authenticated principal resolved by the IAM middleware for the current request.</summary>
public sealed record CurrentUser(int Id, string Email, Role Role, IReadOnlySet<Permission> Permissions)
{
    public bool IsAdmin => Role == Role.ADMIN;
    public bool Has(Permission permission) => Permissions.Contains(permission);
}
