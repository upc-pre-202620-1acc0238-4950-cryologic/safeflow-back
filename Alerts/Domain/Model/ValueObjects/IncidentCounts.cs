namespace SafeFlow.API.Alerts.Domain.Model.ValueObjects;

public sealed record IncidentCounts(int Active, int ActiveCritical, int OtherActive);
