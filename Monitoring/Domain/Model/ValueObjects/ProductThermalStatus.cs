using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Domain.Model.ValueObjects;

public sealed record ProductThermalStatus(int ProductId, int SensorId, double? Temperature,
    ThermalCondition Condition, DateTimeOffset? RecordedAt)
{
    public static IReadOnlyList<ProductThermalStatus> FromSensors(IEnumerable<SensorStatus> sensors) =>
        sensors.GroupBy(status => status.Sensor.ProductId)
            .Select(group => group.OrderByDescending(status => Severity(status.Condition))
                .ThenByDescending(status => status.LatestReading?.RecordedAt)
                .ThenByDescending(status => status.LatestReading?.Id)
                .ThenBy(status => status.Sensor.Id).First())
            .Where(status => status.Condition != ThermalCondition.UNKNOWN)
            .OrderBy(status => status.Sensor.ProductId)
            .Select(status => new ProductThermalStatus(status.Sensor.ProductId, status.Sensor.Id,
                status.LatestReading?.Temperature, status.Condition, status.LatestReading?.RecordedAt))
            .ToList();

    private static int Severity(ThermalCondition condition) => condition switch
    {
        ThermalCondition.DISCONNECTED => 5,
        ThermalCondition.CRITICAL => 4,
        ThermalCondition.RISK => 3,
        ThermalCondition.WARNING => 2,
        ThermalCondition.NORMAL => 1,
        _ => 0
    };
}
