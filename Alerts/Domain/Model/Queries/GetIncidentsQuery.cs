using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Alerts.Domain.Model.Queries;

public sealed record GetIncidentsQuery(bool? Active = null, ThermalCondition? Severity = null);
