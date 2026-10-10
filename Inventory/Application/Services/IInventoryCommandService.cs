using SafeFlow.API.Inventory.Application.Errors;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Commands;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Inventory.Application.Services;

public interface IInventoryCommandService
{
    Task<Result<Product, InventoryError>> HandleAsync(CreateProductCommand command, CancellationToken ct = default);
    Task<Result<Lot, InventoryError>> HandleAsync(CreateLotCommand command, CancellationToken ct = default);
    Task<Result<Income, InventoryError>> HandleAsync(RecordIncomeCommand command, CancellationToken ct = default);
}
