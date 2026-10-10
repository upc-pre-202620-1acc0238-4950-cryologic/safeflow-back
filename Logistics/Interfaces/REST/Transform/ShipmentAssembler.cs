using SafeFlow.API.Logistics.Application.Services;
using SafeFlow.API.Logistics.Domain.Model.Aggregates;
using SafeFlow.API.Logistics.Domain.Model.Commands;
using SafeFlow.API.Logistics.Interfaces.REST.Resources;

namespace SafeFlow.API.Logistics.Interfaces.REST.Transform;

public static class ShipmentAssembler
{
    public static CreateShipmentCommand ToCommand(CreateShipmentResource resource, string by) =>
        new(resource.ProductId, resource.LotId, resource.Quantity, resource.Destination, resource.SensorId, by);

    public static DepartShipmentCommand ToCommand(int shipmentId, DepartureResource resource, string by) =>
        new(shipmentId, resource.AcceptRisk, by);

    public static ShipmentResource ToResource(Shipment shipment, string? productName = null,
        double? temperature = null) =>
        new(shipment.Id, shipment.ProductId, shipment.LotId, shipment.SensorId, shipment.Quantity,
            shipment.Destination, shipment.Status, shipment.CreatedAt, shipment.CreatedBy, shipment.DepartedAt,
            shipment.DepartedBy, shipment.RiskAccepted, shipment.DeliveredAt, shipment.DeliveredBy,
            productName, temperature);

    public static ShipmentResource ToResource(ShipmentDetails details) =>
        ToResource(details.Shipment, details.ProductName, details.Temperature);

    public static ShipmentValidationResource ToResource(ShipmentValidation validation)
    {
        var reading = validation.LatestValidReading;
        var resource = reading is null ? null : new ShipmentReadingResource(reading.Id, reading.SensorId,
            reading.ShipmentId, reading.RecordedAt.ToUniversalTime(), reading.Temperature, reading.Condition);
        return new(validation.SensorConnected, resource, validation.Condition, validation.CanDepart,
            validation.RequiresRiskConfirmation, validation.BlockedReason);
    }
}
