using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Interfaces.REST.Resources;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Transform;

public static class ReadingResourceAssembler
{
    public static ReadingResource ToResource(Reading reading) => new(reading.Id, reading.SensorId,
        reading.ShipmentId, reading.RecordedAt, reading.Temperature, reading.Condition);
}
