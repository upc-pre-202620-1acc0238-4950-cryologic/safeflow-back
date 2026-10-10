using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Queries;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Application.Internal.QueryServices;

public sealed class RolePermissionQueryService(IRolePermissionRepository permissions) : IRolePermissionQueryService
{
    public async Task<IReadOnlySet<Permission>> EffectivePermissionsAsync(Role role, CancellationToken ct = default)
    {
        if (role == Role.ADMIN) return new HashSet<Permission>();
        var granted = await permissions.FindByRoleAsync(role, ct);
        return granted.Select(p => p.Permission).Where(IdentityRules.Defaults(role).Contains).ToHashSet();
    }

    public async Task<IReadOnlyList<RolePermissionsView>> Handle(GetRolePermissionsQuery query, CancellationToken ct = default)
    {
        var result = new List<RolePermissionsView>();
        foreach (var role in new[] { Role.OPERADOR, Role.SUPERVISOR })
            result.Add(new RolePermissionsView(role, await EffectivePermissionsAsync(role, ct)));
        return result;
    }
}
