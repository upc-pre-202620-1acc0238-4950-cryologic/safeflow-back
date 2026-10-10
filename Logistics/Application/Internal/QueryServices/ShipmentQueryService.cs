using SafeFlow.API.Inventory.Interfaces.ACL;
using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Logistics.Application.Services;
using SafeFlow.API.Logistics.Domain.Model.Queries;
using SafeFlow.API.Logistics.Domain.Repositories;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Logistics.Application.Internal.QueryServices;

public sealed class ShipmentQueryService(IShipmentRepository shipments, IInventoryContextFacade inventory,
    IMonitoringContextFacade monitoring, IShipmentValidationService validation) : IShipmentQueryService
{
    public async Task<Result<IReadOnlyList<ShipmentDetails>, LogisticsError>> Handle(GetShipmentsQuery query,
        CancellationToken ct = default)
    {
        var found = await shipments.ListByStatusAsync(query.Status, ct);
        var products = new Dictionary<int, string?>();
        var temperatures = new Dictionary<int, double?>();
        var details = new List<ShipmentDetails>(found.Count);
        // Facades share a scoped DbContext, so their queries must be awaited sequentially.
        foreach (var shipment in found)
        {
            if (!products.ContainsKey(shipment.ProductId))
                products[shipment.ProductId] = (await inventory.FindProductAsync(shipment.ProductId, ct))?.Name;
            if (!temperatures.ContainsKey(shipment.SensorId))
                temperatures[shipment.SensorId] =
                    (await monitoring.LatestValidReadingAsync(shipment.SensorId, ct))?.Temperature;
            details.Add(new(shipment, products[shipment.ProductId], temperatures[shipment.SensorId]));
        }
        return new Result<IReadOnlyList<ShipmentDetails>, LogisticsError>.Success(details);
    }

    public async Task<Result<ShipmentDetails, LogisticsError>> Handle(GetShipmentByIdQuery query,
        CancellationToken ct = default)
    {
        var shipment = await shipments.FindByIdAsync(query.ShipmentId, ct);
        if (shipment is null)
            return new Result<ShipmentDetails, LogisticsError>.Failure(LogisticsError.ShipmentNotFound);
        var product = await inventory.FindProductAsync(shipment.ProductId, ct);
        var reading = await monitoring.LatestValidReadingAsync(shipment.SensorId, ct);
        return new Result<ShipmentDetails, LogisticsError>.Success(new(shipment, product?.Name, reading?.Temperature));
    }

    public async Task<Result<ShipmentValidation, LogisticsError>> Handle(GetShipmentValidationQuery query,
        CancellationToken ct = default)
    {
        var shipment = await shipments.FindByIdAsync(query.ShipmentId, ct);
        if (shipment is null)
            return new Result<ShipmentValidation, LogisticsError>.Failure(LogisticsError.ShipmentNotFound);
        return new Result<ShipmentValidation, LogisticsError>.Success(
            await validation.EvaluateAsync(shipment, DateTimeOffset.UtcNow, ct));
    }
}
