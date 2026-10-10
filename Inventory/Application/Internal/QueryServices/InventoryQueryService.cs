using SafeFlow.API.Inventory.Application.Errors;
using SafeFlow.API.Inventory.Application.Services;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Queries;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Inventory.Application.Internal.QueryServices;

public sealed class InventoryQueryService(
    IProductRepository products,
    ILotRepository lots,
    IMonitoringContextFacade monitoring) : IInventoryQueryService
{
    public async Task<Result<IReadOnlyList<ProductInventory>, InventoryError>> HandleAsync(GetProductsQuery query,
        CancellationToken ct = default)
    {
        var filter = query.Condition?.Trim().ToLowerInvariant();
        if (filter is not null and not "normal" and not "risk")
            return new Result<IReadOnlyList<ProductInventory>, InventoryError>.Failure(InventoryError.InvalidCondition);
        var stock = await products.ListWithStockAsync(ct);
        if (stock.Count == 0)
            return new Result<IReadOnlyList<ProductInventory>, InventoryError>.Success([]);
        var conditions = await monitoring.ProductConditionsAsync(ct);
        IReadOnlyList<ProductInventory> result = stock.Select(totals => new ProductInventory(totals,
                conditions.GetValueOrDefault(totals.Product.Id, ThermalCondition.UNKNOWN)))
            .Where(product => filter switch
            {
                "normal" => product.Condition == ThermalCondition.NORMAL,
                "risk" => product.Condition is ThermalCondition.WARNING or ThermalCondition.RISK
                    or ThermalCondition.CRITICAL or ThermalCondition.DISCONNECTED,
                _ => true
            }).ToList();
        return new Result<IReadOnlyList<ProductInventory>, InventoryError>.Success(result);
    }

    public async Task<Result<ProductInventory, InventoryError>> HandleAsync(GetProductByIdQuery query,
        CancellationToken ct = default)
    {
        var stock = await products.FindWithStockAsync(query.ProductId, ct);
        if (stock is null)
            return new Result<ProductInventory, InventoryError>.Failure(InventoryError.ProductNotFound);
        var conditions = await monitoring.ProductConditionsAsync(ct);
        return new Result<ProductInventory, InventoryError>.Success(new ProductInventory(stock,
            conditions.GetValueOrDefault(query.ProductId, ThermalCondition.UNKNOWN)));
    }

    public async Task<Result<IReadOnlyList<Lot>, InventoryError>> HandleAsync(GetProductLotsQuery query,
        CancellationToken ct = default)
    {
        if (await products.FindByIdAsync(query.ProductId, ct) is null)
            return new Result<IReadOnlyList<Lot>, InventoryError>.Failure(InventoryError.ProductNotFound);
        return new Result<IReadOnlyList<Lot>, InventoryError>.Success(await lots.ListByProductAsync(query.ProductId, ct));
    }

    public async Task<Result<Lot, InventoryError>> HandleAsync(GetLotByIdQuery query, CancellationToken ct = default)
    {
        var lot = await lots.FindByIdAsync(query.LotId, ct);
        return lot is null
            ? new Result<Lot, InventoryError>.Failure(InventoryError.LotNotFound)
            : new Result<Lot, InventoryError>.Success(lot);
    }
}
