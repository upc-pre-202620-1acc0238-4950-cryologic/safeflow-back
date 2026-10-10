using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Resources;

public sealed record ReadingResource(int Id, int SensorId, int? ShipmentId, DateTimeOffset RecordedAt,
    double Temperature, ThermalCondition Condition);
