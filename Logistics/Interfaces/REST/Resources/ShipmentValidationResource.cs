using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Logistics.Interfaces.REST.Resources;

public sealed record ShipmentReadingResource(int Id, int SensorId, int? ShipmentId, DateTimeOffset RecordedAt,
    double Temperature, ThermalCondition Condition);

public sealed record ShipmentValidationResource(bool SensorConnected, ShipmentReadingResource? LatestValidReading,
    ThermalCondition Condition, bool CanDepart, bool RequiresRiskConfirmation, string? BlockedReason);
