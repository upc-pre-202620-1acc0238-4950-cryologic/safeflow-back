using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Application.Services;

public sealed record UserView(int Id, string Email, Role Role, IReadOnlySet<Permission> Permissions);
public sealed record SessionView(UserView User, string Token, DateTimeOffset ExpiresAt);
public sealed record RolePermissionsView(Role Role, IReadOnlySet<Permission> Granted);
