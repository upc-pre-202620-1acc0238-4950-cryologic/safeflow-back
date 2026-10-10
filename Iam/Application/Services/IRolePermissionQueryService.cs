using SafeFlow.API.Iam.Domain.Model.Queries;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Application.Services;

public interface IRolePermissionQueryService
{
    Task<IReadOnlyList<RolePermissionsView>> Handle(GetRolePermissionsQuery query, CancellationToken ct = default);
    Task<IReadOnlySet<Permission>> EffectivePermissionsAsync(Role role, CancellationToken ct = default);
}
