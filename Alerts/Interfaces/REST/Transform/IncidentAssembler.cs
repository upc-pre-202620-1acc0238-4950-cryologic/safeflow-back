using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.Commands;
using SafeFlow.API.Alerts.Interfaces.REST.Resources;

namespace SafeFlow.API.Alerts.Interfaces.REST.Transform;

public static class IncidentAssembler
{
    public static IncidentResource ToResource(Incident incident) => new(incident.Id, incident.SensorId,
        incident.ProductId, incident.Location, incident.Type, incident.Severity, incident.Description,
        incident.OpenedAt, incident.LastSeenAt, incident.Repetitions, incident.Action, incident.Author,
        incident.ResolvedAt, incident.Active);

    public static ResolveIncidentCommand ToCommand(int incidentId, ResolveIncidentResource resource,
        string author) => new(incidentId, resource.Action, author);
}
