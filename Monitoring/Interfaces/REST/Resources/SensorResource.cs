using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Resources;

public sealed record SensorResource(int Id, int ProductId, string Location, bool Connected,
    ThermalCondition Condition, ReadingResource? LatestReading);
