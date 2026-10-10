namespace SafeFlow.API.Logistics.Domain.Model.Commands;

public sealed record DeliverShipmentCommand(int ShipmentId, string DeliveredBy);
