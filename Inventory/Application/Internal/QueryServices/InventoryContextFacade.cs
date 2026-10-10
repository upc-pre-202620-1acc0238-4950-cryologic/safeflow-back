using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Inventory.Interfaces.ACL;

namespace SafeFlow.API.Inventory.Application.Internal.QueryServices;

public sealed class InventoryContextFacade(IProductRepository products, ILotRepository lots) : IInventoryContextFacade
{
    public async Task<ProductSnapshot?> FindProductAsync(int productId, CancellationToken ct = default)
    {
        var product = await products.FindByIdAsync(productId, ct);
        return product is null ? null : ToSnapshot(product);
    }

    public async Task<IReadOnlyList<ProductSnapshot>> ListProductsAsync(CancellationToken ct = default) =>
        (await products.ListAsync(ct)).OrderBy(product => product.Id).Select(ToSnapshot).ToList();

    public async Task<LotSnapshot?> FindLotAsync(int lotId, CancellationToken ct = default)
    {
        var lot = await lots.FindByIdAsync(lotId, ct);
        return lot is null ? null : new LotSnapshot(lot.Id, lot.ProductId, lot.Code, lot.Stock, lot.Reserved);
    }

    public async Task<InventoryTotalsSnapshot> TotalsAsync(CancellationToken ct = default)
    {
        var count = await products.CountAsync(ct);
        var stock = await lots.TotalStockAsync(ct);
        return new InventoryTotalsSnapshot(count, stock);
    }

    public async Task<bool> ReserveAsync(int lotId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0) return false;
        var lot = await lots.FindByIdAsync(lotId, ct);
        return lot is not null && lot.Reserve(quantity);
    }

    public async Task<bool> ConsumeAsync(int lotId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0) return false;
        var lot = await lots.FindByIdAsync(lotId, ct);
        return lot is not null && lot.Consume(quantity);
    }

    private static ProductSnapshot ToSnapshot(Product product) =>
        new(product.Id, product.Name, product.Type, product.Minimum, product.Maximum);
}
