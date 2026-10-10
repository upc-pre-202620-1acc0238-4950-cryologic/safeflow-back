using SafeFlow.API.Logistics.Domain.Model.ValueObjects;
using SafeFlow.API.Logistics.Domain.Repositories;
using SafeFlow.API.Logistics.Interfaces.ACL;

namespace SafeFlow.API.Logistics.Application.Internal.QueryServices;

public sealed class LogisticsContextFacade(IShipmentRepository shipments) : ILogisticsContextFacade
{
    public async Task<ShipmentCountsSnapshot> CountsAsync(CancellationToken ct = default)
    {
        var counts = await shipments.CountsByStatusAsync(ct);
        var preparation = counts.GetValueOrDefault(ShipmentStatus.PREPARATION);
        var transit = counts.GetValueOrDefault(ShipmentStatus.TRANSIT);
        var delivered = counts.GetValueOrDefault(ShipmentStatus.DELIVERED);
        return new(preparation + transit + delivered, preparation, transit, delivered);
    }
}
