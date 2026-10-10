using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Monitoring.Domain.Repositories;

public interface ISensorRepository : IBaseRepository<Sensor>
{
    Task<IReadOnlyList<SensorStatus>> ListWithLatestReadingAsync(int? productId, CancellationToken ct = default);
    Task<SensorStatus?> FindWithLatestReadingAsync(int sensorId, CancellationToken ct = default);
}
