using SafeFlow.API.Alerts.Domain.Model.ValueObjects;

namespace SafeFlow.API.Alerts.Application.Errors;

public abstract record IncidentError(string Code, string Message)
{
    public sealed record NotFound() : IncidentError("incident_not_found", "La incidencia no existe.");
    public sealed record ActionRequired() : IncidentError("corrective_action_required",
        "Describe la acción correctiva para resolver la incidencia.");
    public sealed record AlreadyResolved() : IncidentError("incident_already_resolved",
        "La incidencia ya fue resuelta.");
    public sealed record InvalidSeverity() : IncidentError("invalid_severity",
        "La severidad indicada no es válida.");

    public static IncidentError From(IncidentResolutionException exception) =>
        exception.Failure == IncidentResolutionFailure.ActionRequired
            ? new ActionRequired()
            : new AlreadyResolved();
}
