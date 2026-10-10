using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Shared.Domain.Model;
using SafeFlow.API.Shared.Domain.Services;

namespace SafeFlow.API.Logistics.Application.Services;

public sealed record ShipmentValidation(bool SensorConnected, ReadingSnapshot? LatestValidReading,
    ThermalCondition Condition, LogisticsError? BlockedError)
{
    public bool CanDepart => BlockedError is null;
    public bool RequiresRiskConfirmation => CanDepart && ThermalRules.IsRisk(Condition);
    public string? BlockedReason => BlockedError?.Message;
}
