using SafeFlow.API.Logistics.Domain.Model.Aggregates;

namespace SafeFlow.API.Logistics.Application.Services;

public interface IShipmentValidationService
{
    Task<ShipmentValidation> EvaluateAsync(Shipment shipment, DateTimeOffset at, CancellationToken ct = default);
}
