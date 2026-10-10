using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Monitoring.Domain.Repositories;

public interface IRejectedReadingRepository : IBaseRepository<RejectedReading>
{
    Task<IReadOnlyList<RejectedReading>> FindBySensorAsync(int? sensorId, CancellationToken ct = default);
}
