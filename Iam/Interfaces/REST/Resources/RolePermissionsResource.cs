using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Interfaces.REST.Resources;

public sealed record RolePermissionsResource(Role Role, IReadOnlyList<Permission> Granted);
