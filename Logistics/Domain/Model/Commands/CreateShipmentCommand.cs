namespace SafeFlow.API.Logistics.Domain.Model.Commands;

public sealed record CreateShipmentCommand(int ProductId, int LotId, int Quantity, string? Destination,
    int SensorId, string CreatedBy);
