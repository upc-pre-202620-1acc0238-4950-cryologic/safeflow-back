using SafeFlow.API.Alerts.Domain.Repositories;

namespace SafeFlow.API.Alerts.Interfaces.ACL;

public sealed class AlertsContextFacade(IIncidentRepository repository) : IAlertsContextFacade
{
    public async Task<IncidentCountsSnapshot> CountsAsync(CancellationToken ct = default)
    {
        var counts = await repository.CountsAsync(ct);
        return new IncidentCountsSnapshot(counts.Active, counts.ActiveCritical, counts.OtherActive);
    }
}
