using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Iam.Application.Internal.CommandServices;

public sealed class RolePermissionCommandService(IRolePermissionRepository permissions, IUnitOfWork unitOfWork)
    : IRolePermissionCommandService
{
    public async Task<Result<RolePermissionsView, IamError>> Handle(
        ReplaceRolePermissionsCommand command, CancellationToken ct = default)
    {
        if (command.Role == Role.ADMIN)
            return new Result<RolePermissionsView, IamError>.Failure(IamError.AdminPermissions);
        if (!Enum.IsDefined(command.Role) || command.Granted is null
            || !command.Granted.All(IdentityRules.Defaults(command.Role).Contains))
            return new Result<RolePermissionsView, IamError>.Failure(IamError.InvalidPermissions);

        var existing = await permissions.FindByRoleAsync(command.Role, ct);
        foreach (var permission in existing.Where(p => !command.Granted.Contains(p.Permission)))
            permissions.Remove(permission);
        var alreadyGranted = existing.Select(p => p.Permission).ToHashSet();
        foreach (var permission in command.Granted.Where(p => !alreadyGranted.Contains(p)))
            await permissions.AddAsync(RolePermission.Grant(command.Role, permission), ct);

        await unitOfWork.CompleteAsync(ct);
        return new Result<RolePermissionsView, IamError>.Success(
            new RolePermissionsView(command.Role, command.Granted.ToHashSet()));
    }
}
