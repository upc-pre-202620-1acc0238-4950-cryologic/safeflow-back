using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Repositories;

public sealed class RolePermissionRepository(AppDbContext context)
    : BaseRepository<RolePermission>(context), IRolePermissionRepository
{
    public async Task<IReadOnlyList<RolePermission>> FindByRoleAsync(Role role, CancellationToken ct = default) =>
        await Context.Set<RolePermission>().Where(p => p.Role == role).ToListAsync(ct);
}
