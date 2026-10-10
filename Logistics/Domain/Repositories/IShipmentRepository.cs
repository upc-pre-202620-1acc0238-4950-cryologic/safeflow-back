using SafeFlow.API.Logistics.Domain.Model.Aggregates;
using SafeFlow.API.Logistics.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Logistics.Domain.Repositories;

public interface IShipmentRepository : IBaseRepository<Shipment>
{
    Task<IReadOnlyList<Shipment>> ListByStatusAsync(ShipmentStatus? status, CancellationToken ct = default);
    Task<bool> HasActiveShipmentAsync(int sensorId, CancellationToken ct = default);
    Task<IReadOnlyDictionary<ShipmentStatus, int>> CountsByStatusAsync(CancellationToken ct = default);
}
