using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Domain.Model.ValueObjects;

public sealed record SensorStatus(Sensor Sensor, Reading? LatestReading)
{
    public ThermalCondition Condition => !Sensor.Connected
        ? ThermalCondition.DISCONNECTED
        : LatestReading?.Condition ?? ThermalCondition.UNKNOWN;
}
