namespace SafeFlow.API.Alerts.Interfaces.ACL;

public sealed record IncidentCountsSnapshot(int Active, int ActiveCritical, int ActiveWarnings);

/// <summary>Open host service of Alerts (read only).</summary>
public interface IAlertsContextFacade
{
    Task<IncidentCountsSnapshot> CountsAsync(CancellationToken ct = default);
}
