using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Inventory.Domain.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IReadOnlyList<ProductStock>> ListWithStockAsync(CancellationToken ct = default);
    Task<ProductStock?> FindWithStockAsync(int productId, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}
