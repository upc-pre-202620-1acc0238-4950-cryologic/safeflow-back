using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Monitoring.Interfaces.REST.Resources;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Transform;

public static class SensorResourceAssembler
{
    public static SensorResource ToResource(SensorStatus status) => new(status.Sensor.Id, status.Sensor.ProductId,
        status.Sensor.Location, status.Sensor.Connected, status.Condition,
        status.LatestReading is null ? null : ReadingResourceAssembler.ToResource(status.LatestReading));
}
