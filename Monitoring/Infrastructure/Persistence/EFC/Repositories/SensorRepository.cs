using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Monitoring.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public sealed class SensorRepository(AppDbContext context) : BaseRepository<Sensor>(context), ISensorRepository
{
    public async Task<IReadOnlyList<SensorStatus>> ListWithLatestReadingAsync(int? productId,
        CancellationToken ct = default)
    {
        var sensors = Context.Set<Sensor>().AsNoTracking();
        if (productId.HasValue)
            sensors = sensors.Where(sensor => sensor.ProductId == productId.Value);

        return await WithLatestReading(sensors).ToListAsync(ct);
    }

    public Task<SensorStatus?> FindWithLatestReadingAsync(int sensorId, CancellationToken ct = default) =>
        WithLatestReading(Context.Set<Sensor>().AsNoTracking().Where(sensor => sensor.Id == sensorId))
            .FirstOrDefaultAsync(ct);

    private IQueryable<SensorStatus> WithLatestReading(IQueryable<Sensor> sensors)
    {
        var readings = Context.Set<Reading>().AsNoTracking();
        // NOT EXISTS avoids loading historical readings and resolves equal timestamps by id.
        var latestReadings = readings.Where(reading => !readings.Any(newer =>
            newer.SensorId == reading.SensorId && (newer.RecordedAt > reading.RecordedAt ||
                (newer.RecordedAt == reading.RecordedAt && newer.Id > reading.Id))));

        return from sensor in sensors
            join reading in latestReadings on sensor.Id equals reading.SensorId into latest
            from reading in latest.DefaultIfEmpty()
            orderby sensor.Id
            select new SensorStatus(sensor, reading);
    }
}
