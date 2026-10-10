using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Monitoring.Interfaces.REST.Resources;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Transform;

public static class RiskResourceAssembler
{
    public static RiskResource ToResource(ProductThermalStatus status) => new(status.ProductId,
        status.SensorId, status.Temperature, status.Condition, status.RecordedAt);
}
