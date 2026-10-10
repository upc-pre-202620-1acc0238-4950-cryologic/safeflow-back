using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Repositories;

public sealed class RevokedTokenRepository(AppDbContext context)
    : BaseRepository<RevokedToken>(context), IRevokedTokenRepository
{
    public Task<bool> ContainsAsync(string jti, CancellationToken ct = default) =>
        Context.Set<RevokedToken>().AnyAsync(t => t.Jti == jti, ct);
}
