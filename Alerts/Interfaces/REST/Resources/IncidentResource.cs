using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Alerts.Interfaces.REST.Resources;

public sealed record IncidentResource(int Id, int SensorId, int ProductId, string Location, string Type,
    ThermalCondition Severity, string Description, DateTimeOffset OpenedAt, DateTimeOffset LastSeenAt,
    int Repetitions, string? Action, string? Author, DateTimeOffset? ResolvedAt, bool Active);
