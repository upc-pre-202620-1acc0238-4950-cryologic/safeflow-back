using SafeFlow.API.Inventory.Application.Errors;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Queries;
using SafeFlow.API.Inventory.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Inventory.Application.Services;

public sealed record ProductInventory(ProductStock Totals, ThermalCondition Condition);

public interface IInventoryQueryService
{
    Task<Result<IReadOnlyList<ProductInventory>, InventoryError>> HandleAsync(GetProductsQuery query,
        CancellationToken ct = default);
    Task<Result<ProductInventory, InventoryError>> HandleAsync(GetProductByIdQuery query,
        CancellationToken ct = default);
    Task<Result<IReadOnlyList<Lot>, InventoryError>> HandleAsync(GetProductLotsQuery query,
        CancellationToken ct = default);
    Task<Result<Lot, InventoryError>> HandleAsync(GetLotByIdQuery query, CancellationToken ct = default);
}
