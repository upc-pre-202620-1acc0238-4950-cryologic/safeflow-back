namespace SafeFlow.API.Alerts.Domain.Model.Commands;

public sealed record ResolveIncidentCommand(int IncidentId, string? Action, string Author);
