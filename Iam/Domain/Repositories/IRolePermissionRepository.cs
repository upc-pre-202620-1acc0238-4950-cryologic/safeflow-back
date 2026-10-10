using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Shared.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Domain.Repositories;

public interface IRolePermissionRepository : IBaseRepository<RolePermission>
{
    Task<IReadOnlyList<RolePermission>> FindByRoleAsync(Role role, CancellationToken ct = default);
}
