using SafeFlow.API.Logistics.Domain.Model.ValueObjects;

namespace SafeFlow.API.Logistics.Interfaces.REST.Resources;

public sealed record ShipmentResource(int Id, int ProductId, int LotId, int SensorId, int Quantity,
    string Destination, ShipmentStatus Status, DateTimeOffset CreatedAt, string CreatedBy,
    DateTimeOffset? DepartedAt, string? DepartedBy, bool RiskAccepted, DateTimeOffset? DeliveredAt,
    string? DeliveredBy, string? ProductName, double? Temperature);
