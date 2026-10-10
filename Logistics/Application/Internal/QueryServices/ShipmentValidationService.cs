using SafeFlow.API.Inventory.Interfaces.ACL;
using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Logistics.Application.Services;
using SafeFlow.API.Logistics.Domain.Model.Aggregates;
using SafeFlow.API.Logistics.Domain.Model.ValueObjects;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Shared.Domain.Model;
using SafeFlow.API.Shared.Domain.Services;

namespace SafeFlow.API.Logistics.Application.Internal.QueryServices;

public sealed class ShipmentValidationService(IInventoryContextFacade inventory, IMonitoringContextFacade monitoring)
    : IShipmentValidationService
{
    public async Task<ShipmentValidation> EvaluateAsync(Shipment shipment, DateTimeOffset at,
        CancellationToken ct = default)
    {
        var sensor = await monitoring.FindSensorAsync(shipment.SensorId, ct);
        var reading = await monitoring.LatestValidReadingAsync(shipment.SensorId, ct);
        var connected = sensor?.Connected == true;
        // The mock accepts exactly five minutes; the ACL only exposes valid measurements.
        var current = reading is not null && at - reading.RecordedAt <= TimeSpan.FromMinutes(5);
        var product = connected && current ? await inventory.FindProductAsync(shipment.ProductId, ct) : null;
        var condition = product is not null && reading is not null
            ? ThermalRules.Evaluate(reading.Temperature, product.Minimum, product.Maximum)
            : ThermalCondition.UNKNOWN;

        LogisticsError? blocked = shipment.Status != ShipmentStatus.PREPARATION ? LogisticsError.AlreadyDeparted
            : !connected ? LogisticsError.SensorDisconnected
            : !current ? LogisticsError.ReadingUnavailable
            : product is null ? LogisticsError.ProductNotFound
            : null;
        return new(connected, reading, condition, blocked);
    }
}
