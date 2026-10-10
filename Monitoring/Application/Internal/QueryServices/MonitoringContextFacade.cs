using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Monitoring.Domain.Repositories;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Application.Internal.QueryServices;

public sealed class MonitoringContextFacade(ISensorRepository sensors, IReadingRepository readings)
    : IMonitoringContextFacade
{
    public async Task<SensorSnapshot?> FindSensorAsync(int sensorId, CancellationToken ct = default)
    {
        var status = await sensors.FindWithLatestReadingAsync(sensorId, ct);
        return status is null ? null : new SensorSnapshot(status.Sensor.Id, status.Sensor.ProductId,
            status.Sensor.Location, status.Sensor.Connected);
    }

    public async Task<ReadingSnapshot?> LatestValidReadingAsync(int sensorId, CancellationToken ct = default)
    {
        var reading = await readings.LatestForSensorAsync(sensorId, ct);
        return reading is null ? null : new ReadingSnapshot(reading.Id, reading.SensorId, reading.ShipmentId,
            reading.RecordedAt, reading.Temperature, reading.Condition);
    }

    public async Task<IReadOnlyDictionary<int, ThermalCondition>> ProductConditionsAsync(CancellationToken ct = default)
    {
        var statuses = await sensors.ListWithLatestReadingAsync(null, ct);
        // The ACL contract omits products without valid readings so callers can treat them as UNKNOWN.
        var measuredProducts = statuses.GroupBy(status => status.Sensor.ProductId)
            .Where(group => group.Any(status => status.LatestReading is not null)).SelectMany(group => group);
        return ProductThermalStatus.FromSensors(measuredProducts)
            .ToDictionary(status => status.ProductId, status => status.Condition);
    }
}
