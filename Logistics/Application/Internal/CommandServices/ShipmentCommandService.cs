using SafeFlow.API.Inventory.Interfaces.ACL;
using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Logistics.Application.Services;
using SafeFlow.API.Logistics.Domain.Model.Aggregates;
using SafeFlow.API.Logistics.Domain.Model.Commands;
using SafeFlow.API.Logistics.Domain.Model.ValueObjects;
using SafeFlow.API.Logistics.Domain.Repositories;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Logistics.Application.Internal.CommandServices;

public sealed class ShipmentCommandService(IShipmentRepository shipments, IInventoryContextFacade inventory,
    IMonitoringContextFacade monitoring, IShipmentValidationService validation, IUnitOfWork unitOfWork)
    : IShipmentCommandService
{
    public async Task<Result<Shipment, LogisticsError>> Handle(CreateShipmentCommand command,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Destination)) return Failure(LogisticsError.DestinationRequired);
        if (command.Quantity <= 0) return Failure(LogisticsError.InvalidQuantity);
        var product = await inventory.FindProductAsync(command.ProductId, ct);
        if (product is null) return Failure(LogisticsError.SelectedProductNotFound);
        var lot = await inventory.FindLotAsync(command.LotId, ct);
        if (lot is null) return Failure(LogisticsError.SelectedLotNotFound);
        if (lot.ProductId != product.Id) return Failure(LogisticsError.LotProductMismatch);
        if (command.Quantity > lot.Available) return Failure(LogisticsError.InsufficientStock(lot.Available));
        var sensor = await monitoring.FindSensorAsync(command.SensorId, ct);
        if (sensor is null) return Failure(LogisticsError.SensorNotFound);
        if (sensor.ProductId != product.Id) return Failure(LogisticsError.SensorProductMismatch);
        if (await shipments.HasActiveShipmentAsync(command.SensorId, ct)) return Failure(LogisticsError.SensorInUse);

        var shipment = Shipment.Create(command, DateTimeOffset.UtcNow);
        if (!await inventory.ReserveAsync(command.LotId, command.Quantity, ct))
        {
            var currentLot = await inventory.FindLotAsync(command.LotId, ct);
            return Failure(currentLot is null ? LogisticsError.SelectedLotNotFound :
                LogisticsError.InsufficientStock(currentLot.Available));
        }
        await shipments.AddAsync(shipment, ct);
        // Inventory only stages its changes in the same scoped unit of work.
        await unitOfWork.CompleteAsync(ct);
        return new Result<Shipment, LogisticsError>.Success(shipment);
    }

    public async Task<Result<Shipment, LogisticsError>> Handle(DepartShipmentCommand command,
        CancellationToken ct = default)
    {
        var shipment = await shipments.FindByIdAsync(command.ShipmentId, ct);
        if (shipment is null) return Failure(LogisticsError.ShipmentNotFound);
        if (shipment.Status != ShipmentStatus.PREPARATION) return Failure(LogisticsError.AlreadyDeparted);
        var at = DateTimeOffset.UtcNow;
        var assessment = await validation.EvaluateAsync(shipment, at, ct);
        if (assessment.BlockedError is not null) return Failure(assessment.BlockedError);
        if (assessment.RequiresRiskConfirmation && !command.AcceptRisk)
            return Failure(LogisticsError.RiskConfirmationRequired);

        shipment.Depart(assessment.RequiresRiskConfirmation, command.DepartedBy, at);
        await unitOfWork.CompleteAsync(ct);
        return new Result<Shipment, LogisticsError>.Success(shipment);
    }

    public async Task<Result<Shipment, LogisticsError>> Handle(DeliverShipmentCommand command,
        CancellationToken ct = default)
    {
        var shipment = await shipments.FindByIdAsync(command.ShipmentId, ct);
        if (shipment is null) return Failure(LogisticsError.ShipmentNotFound);
        if (shipment.Status == ShipmentStatus.DELIVERED) return Failure(LogisticsError.AlreadyDelivered);
        if (shipment.Status != ShipmentStatus.TRANSIT) return Failure(LogisticsError.NotInTransit);
        if (await inventory.FindProductAsync(shipment.ProductId, ct) is null)
            return Failure(LogisticsError.ProductNotFound);
        var lot = await inventory.FindLotAsync(shipment.LotId, ct);
        if (lot is null) return Failure(LogisticsError.LotNotFound);
        if (lot.ProductId != shipment.ProductId || shipment.Quantity <= 0 || lot.Reserved < shipment.Quantity ||
            lot.Stock < shipment.Quantity)
            return Failure(LogisticsError.InconsistentReservation);
        if (!await inventory.ConsumeAsync(shipment.LotId, shipment.Quantity, ct))
            return Failure(LogisticsError.InconsistentReservation);

        shipment.Deliver(command.DeliveredBy, DateTimeOffset.UtcNow);
        await unitOfWork.CompleteAsync(ct);
        return new Result<Shipment, LogisticsError>.Success(shipment);
    }

    private static Result<Shipment, LogisticsError> Failure(LogisticsError error) =>
        new Result<Shipment, LogisticsError>.Failure(error);
}
