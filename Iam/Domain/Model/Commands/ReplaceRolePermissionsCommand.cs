using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Domain.Model.Commands;

public sealed record ReplaceRolePermissionsCommand(Role Role, IReadOnlySet<Permission> Granted);
