using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Model.Queries;
using SafeFlow.API.Monitoring.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public sealed class ReadingRepository(AppDbContext context) : BaseRepository<Reading>(context), IReadingRepository
{
    public async Task<IReadOnlyList<Reading>> FindAsync(GetReadingsQuery query, CancellationToken ct = default)
    {
        var readings = Context.Set<Reading>().AsNoTracking();
        if (query.ProductId.HasValue)
        {
            var sensorIds = Context.Set<Sensor>().Where(sensor => sensor.ProductId == query.ProductId.Value)
                .Select(sensor => sensor.Id);
            readings = readings.Where(reading => sensorIds.Contains(reading.SensorId));
        }
        if (query.SensorId.HasValue)
            readings = readings.Where(reading => reading.SensorId == query.SensorId.Value);
        if (query.ShipmentId.HasValue)
            readings = readings.Where(reading => reading.ShipmentId == query.ShipmentId.Value);
        if (query.From.HasValue)
            readings = readings.Where(reading => reading.RecordedAt >= query.From.Value);
        if (query.To.HasValue)
            readings = readings.Where(reading => reading.RecordedAt <= query.To.Value);

        return await readings.OrderBy(reading => reading.RecordedAt).ThenBy(reading => reading.Id).ToListAsync(ct);
    }

    public Task<Reading?> LatestForSensorAsync(int sensorId, CancellationToken ct = default) =>
        Context.Set<Reading>().AsNoTracking().Where(reading => reading.SensorId == sensorId)
            .OrderByDescending(reading => reading.RecordedAt).ThenByDescending(reading => reading.Id)
            .FirstOrDefaultAsync(ct);
}
