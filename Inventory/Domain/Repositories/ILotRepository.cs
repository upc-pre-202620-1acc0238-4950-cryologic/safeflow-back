using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Inventory.Domain.Repositories;

public interface ILotRepository : IBaseRepository<Lot>
{
    Task<Lot?> FindByCodeAsync(int productId, string code, CancellationToken ct = default);
    Task<IReadOnlyList<Lot>> ListByProductAsync(int productId, CancellationToken ct = default);
    Task<long> TotalStockAsync(CancellationToken ct = default);
}
