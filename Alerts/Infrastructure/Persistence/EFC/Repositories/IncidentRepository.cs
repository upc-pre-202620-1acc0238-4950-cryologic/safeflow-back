using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.ValueObjects;
using SafeFlow.API.Alerts.Domain.Repositories;
using SafeFlow.API.Shared.Domain.Model;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Alerts.Infrastructure.Persistence.EFC.Repositories;

public sealed class IncidentRepository(AppDbContext context) : BaseRepository<Incident>(context), IIncidentRepository
{
    public async Task<IReadOnlyList<Incident>> FindAllAsync(bool? active, ThermalCondition? severity,
        CancellationToken ct = default)
    {
        var query = Context.Set<Incident>().AsNoTracking();
        if (active is { } isActive)
            query = isActive
                ? query.Where(incident => incident.ResolvedAt == null)
                : query.Where(incident => incident.ResolvedAt != null);
        if (severity is { } condition)
            query = query.Where(incident => incident.Severity == condition);

        return await query
            .OrderBy(incident => incident.Severity == ThermalCondition.CRITICAL ? 0
                : incident.Severity == ThermalCondition.DISCONNECTED ? 1
                : incident.Severity == ThermalCondition.RISK ? 2
                : incident.Severity == ThermalCondition.WARNING ? 3
                : incident.Severity == ThermalCondition.NORMAL ? 4 : 5)
            .ThenByDescending(incident => incident.LastSeenAt)
            .ThenBy(incident => incident.Id)
            .ToListAsync(ct);
    }

    public async Task<IncidentCounts> CountsAsync(CancellationToken ct = default)
    {
        var counts = await Context.Set<Incident>().AsNoTracking()
            .Where(incident => incident.ResolvedAt == null)
            .GroupBy(incident => 1)
            .Select(group => new IncidentCounts(group.Count(),
                group.Count(incident => incident.Severity == ThermalCondition.CRITICAL),
                group.Count(incident => incident.Severity != ThermalCondition.CRITICAL)))
            .SingleOrDefaultAsync(ct);
        return counts ?? new IncidentCounts(0, 0, 0);
    }
}
