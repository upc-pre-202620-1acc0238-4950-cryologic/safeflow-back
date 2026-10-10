namespace SafeFlow.API.Logistics.Interfaces.ACL;

public sealed record ShipmentCountsSnapshot(int Total, int Preparation, int Transit, int Delivered);

/// <summary>Open host service of Logistics (read only).</summary>
public interface ILogisticsContextFacade
{
    Task<ShipmentCountsSnapshot> CountsAsync(CancellationToken ct = default);
}
