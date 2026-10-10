using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Iam.Interfaces.REST.Resources;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Interfaces.REST.Transform;

public static class RolePermissionAssembler
{
    public static ReplaceRolePermissionsCommand ToCommand(Role role, ReplaceRolePermissionsResource resource) =>
        new(role, resource.Granted!);

    public static RolePermissionsResource ToResource(RolePermissionsView permissions) =>
        new(permissions.Role, permissions.Granted.OrderBy(p => p).ToArray());
}
