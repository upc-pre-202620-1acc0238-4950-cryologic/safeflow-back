using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Model;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Alerts.Domain.Repositories;

public interface IIncidentRepository : IBaseRepository<Incident>
{
    Task<IReadOnlyList<Incident>> FindAllAsync(bool? active, ThermalCondition? severity,
        CancellationToken ct = default);
    Task<IncidentCounts> CountsAsync(CancellationToken ct = default);
}
