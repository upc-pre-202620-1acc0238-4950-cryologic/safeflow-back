using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Interfaces.REST.Resources;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Transform;

public static class RejectedReadingResourceAssembler
{
    public static RejectedReadingResource ToResource(RejectedReading reading) => new(reading.Id,
        reading.SensorId, reading.RecordedAt, reading.Raw, reading.Reason);
}
