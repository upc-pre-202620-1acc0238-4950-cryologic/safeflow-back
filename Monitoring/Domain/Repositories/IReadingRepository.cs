using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Model.Queries;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Monitoring.Domain.Repositories;

public interface IReadingRepository : IBaseRepository<Reading>
{
    Task<IReadOnlyList<Reading>> FindAsync(GetReadingsQuery query, CancellationToken ct = default);
    Task<Reading?> LatestForSensorAsync(int sensorId, CancellationToken ct = default);
}
