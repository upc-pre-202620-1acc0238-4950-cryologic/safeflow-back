using SafeFlow.API.Logistics.Domain.Model.ValueObjects;

namespace SafeFlow.API.Logistics.Domain.Model.Queries;

public sealed record GetShipmentsQuery(ShipmentStatus? Status = null);
