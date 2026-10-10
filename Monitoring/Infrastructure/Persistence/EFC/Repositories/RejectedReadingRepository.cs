using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public sealed class RejectedReadingRepository(AppDbContext context) : BaseRepository<RejectedReading>(context),
    IRejectedReadingRepository
{
    public async Task<IReadOnlyList<RejectedReading>> FindBySensorAsync(int? sensorId, CancellationToken ct = default)
    {
        var readings = Context.Set<RejectedReading>().AsNoTracking();
        if (sensorId.HasValue)
            readings = readings.Where(reading => reading.SensorId == sensorId.Value);

        return await readings.OrderBy(reading => reading.RecordedAt).ThenBy(reading => reading.Id).ToListAsync(ct);
    }
}
