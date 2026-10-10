using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Interfaces.REST.Resources;

public sealed record SessionResource(int Id, string Email, Role Role, IReadOnlyList<Permission> Permissions,
    string Token, DateTimeOffset ExpiresAt);
