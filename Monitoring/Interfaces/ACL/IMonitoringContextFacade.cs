using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Interfaces.ACL;

public sealed record SensorSnapshot(int Id, int ProductId, string Location, bool Connected);

public sealed record ReadingSnapshot(int Id, int SensorId, int? ShipmentId, DateTimeOffset RecordedAt,
    double Temperature, ThermalCondition Condition);

/// <summary>Open host service of Monitoring (read only). Readings are valid measurements only.</summary>
public interface IMonitoringContextFacade
{
    Task<SensorSnapshot?> FindSensorAsync(int sensorId, CancellationToken ct = default);
    Task<ReadingSnapshot?> LatestValidReadingAsync(int sensorId, CancellationToken ct = default);

    /// <summary>
    /// Current condition per product id: condition of its latest valid reading, DISCONNECTED when its
    /// sensor is disconnected, products without readings are omitted (UNKNOWN for callers).
    /// </summary>
    Task<IReadOnlyDictionary<int, ThermalCondition>> ProductConditionsAsync(CancellationToken ct = default);
}
