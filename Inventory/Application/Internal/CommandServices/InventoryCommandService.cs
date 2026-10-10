using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using SafeFlow.API.Inventory.Application.Errors;
using SafeFlow.API.Inventory.Application.Services;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Commands;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Inventory.Application.Internal.CommandServices;

public sealed class InventoryCommandService(
    IProductRepository products,
    ILotRepository lots,
    IIncomeRepository incomes,
    IUnitOfWork unitOfWork) : IInventoryCommandService
{
    public async Task<Result<Product, InventoryError>> HandleAsync(CreateProductCommand command,
        CancellationToken ct = default)
    {
        Product product;
        try
        {
            product = Product.Create(command);
        }
        catch (ArgumentException exception)
        {
            return new Result<Product, InventoryError>.Failure(InventoryError.InvalidProduct(exception.Message));
        }
        await products.AddAsync(product, ct);
        await unitOfWork.CompleteAsync(ct);
        return new Result<Product, InventoryError>.Success(product);
    }

    public async Task<Result<Lot, InventoryError>> HandleAsync(CreateLotCommand command,
        CancellationToken ct = default)
    {
        if (await products.FindByIdAsync(command.ProductId, ct) is null)
            return new Result<Lot, InventoryError>.Failure(InventoryError.ProductNotFound);
        Lot lot;
        try
        {
            lot = Lot.Create(command);
        }
        catch (ArgumentException exception)
        {
            return new Result<Lot, InventoryError>.Failure(InventoryError.InvalidLot(exception.Message));
        }
        var duplicate = await lots.FindByCodeAsync(lot.ProductId, lot.Code, ct);
        if (duplicate is not null)
            return new Result<Lot, InventoryError>.Failure(InventoryError.DuplicateLot(lot.Code, duplicate.Id));

        await lots.AddAsync(lot, ct);
        try
        {
            await unitOfWork.CompleteAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.GetBaseException() is MySqlException { Number: 1062 })
        {
            // The unique index also protects two requests that passed the preceding check together.
            duplicate = await lots.FindByCodeAsync(lot.ProductId, lot.Code, ct);
            if (duplicate is null) throw;
            return new Result<Lot, InventoryError>.Failure(InventoryError.DuplicateLot(lot.Code, duplicate.Id));
        }
        return new Result<Lot, InventoryError>.Success(lot);
    }

    public async Task<Result<Income, InventoryError>> HandleAsync(RecordIncomeCommand command,
        CancellationToken ct = default)
    {
        if (command.Quantity is not > 0)
            return new Result<Income, InventoryError>.Failure(InventoryError.InvalidIncome(
                "La cantidad debe ser un entero mayor que cero."));
        var lot = await lots.FindByIdAsync(command.LotId, ct);
        if (lot is null)
            return new Result<Income, InventoryError>.Failure(InventoryError.LotNotFound);
        if (await products.FindByIdAsync(lot.ProductId, ct) is null)
            return new Result<Income, InventoryError>.Failure(InventoryError.MissingLotProduct);
        Income income;
        try
        {
            income = Income.Record(command, lot, DateTimeOffset.UtcNow);
            lot.AddIncome(income.Quantity);
        }
        catch (ArgumentException exception)
        {
            return new Result<Income, InventoryError>.Failure(InventoryError.InvalidIncome(exception.Message));
        }
        catch (OverflowException exception)
        {
            return new Result<Income, InventoryError>.Failure(InventoryError.InvalidIncome(exception.Message));
        }
        await incomes.AddAsync(income, ct);
        try
        {
            // EF commits both the income and the tracked lot update in the same transaction.
            await unitOfWork.CompleteAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Result<Income, InventoryError>.Failure(InventoryError.ConcurrentLotChange);
        }
        return new Result<Income, InventoryError>.Success(income);
    }
}
