using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Repositories;

public sealed class LotRepository(AppDbContext context) : BaseRepository<Lot>(context), ILotRepository
{
    public Task<Lot?> FindByCodeAsync(int productId, string code, CancellationToken ct = default) =>
        Context.Set<Lot>().AsNoTracking()
            .SingleOrDefaultAsync(lot => lot.ProductId == productId && lot.Code == code.Trim(), ct);

    public async Task<IReadOnlyList<Lot>> ListByProductAsync(int productId, CancellationToken ct = default) =>
        await Context.Set<Lot>().AsNoTracking().Where(lot => lot.ProductId == productId)
            .OrderBy(lot => lot.Id).ToListAsync(ct);

    public async Task<long> TotalStockAsync(CancellationToken ct = default) =>
        await Context.Set<Lot>().SumAsync(lot => (long?)lot.Stock, ct) ?? 0;
}
