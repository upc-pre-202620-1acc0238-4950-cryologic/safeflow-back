using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Iam.Application.Services;

public interface IRolePermissionCommandService
{
    Task<Result<RolePermissionsView, IamError>> Handle(ReplaceRolePermissionsCommand command, CancellationToken ct = default);
}
