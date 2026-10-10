using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Repositories;

public sealed class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        Context.Set<User>().SingleOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> AnyByRoleAsync(Role role, CancellationToken ct = default) =>
        Context.Set<User>().AnyAsync(u => u.Role == role, ct);
}
