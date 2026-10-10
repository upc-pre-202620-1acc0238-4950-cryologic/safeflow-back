using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Logistics.Domain.Model.Aggregates;
using SafeFlow.API.Logistics.Domain.Model.ValueObjects;
using SafeFlow.API.Logistics.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Logistics.Infrastructure.Persistence.EFC.Repositories;

public sealed class ShipmentRepository(AppDbContext context) : BaseRepository<Shipment>(context), IShipmentRepository
{
    public async Task<IReadOnlyList<Shipment>> ListByStatusAsync(ShipmentStatus? status,
        CancellationToken ct = default)
    {
        var query = Context.Set<Shipment>().AsNoTracking();
        if (status.HasValue) query = query.Where(shipment => shipment.Status == status.Value);
        return await query.OrderByDescending(shipment => shipment.CreatedAt)
            .ThenByDescending(shipment => shipment.Id).ToListAsync(ct);
    }

    public Task<bool> HasActiveShipmentAsync(int sensorId, CancellationToken ct = default) =>
        Context.Set<Shipment>().AnyAsync(shipment => shipment.SensorId == sensorId &&
            shipment.Status != ShipmentStatus.DELIVERED, ct);

    public async Task<IReadOnlyDictionary<ShipmentStatus, int>> CountsByStatusAsync(CancellationToken ct = default) =>
        await Context.Set<Shipment>().AsNoTracking().GroupBy(shipment => shipment.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionaryAsync(group => group.Status, group => group.Count, ct);
}
