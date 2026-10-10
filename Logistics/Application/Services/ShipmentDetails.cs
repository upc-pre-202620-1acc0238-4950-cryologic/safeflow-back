using SafeFlow.API.Logistics.Domain.Model.Aggregates;

namespace SafeFlow.API.Logistics.Application.Services;

public sealed record ShipmentDetails(Shipment Shipment, string? ProductName, double? Temperature);
