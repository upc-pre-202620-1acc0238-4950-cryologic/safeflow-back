using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Shared.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> AnyByRoleAsync(Role role, CancellationToken ct = default);
}
