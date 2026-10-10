using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Iam.Domain.Repositories;

public interface IRevokedTokenRepository : IBaseRepository<RevokedToken>
{
    Task<bool> ContainsAsync(string jti, CancellationToken ct = default);
}
