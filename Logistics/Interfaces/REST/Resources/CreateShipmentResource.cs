namespace SafeFlow.API.Logistics.Interfaces.REST.Resources;

public sealed record CreateShipmentResource(int ProductId, int LotId, int Quantity, string? Destination, int SensorId);
