using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Resources;

public sealed record RiskResource(int ProductId, int SensorId, double? Temperature,
    ThermalCondition Condition, DateTimeOffset? RecordedAt);
