namespace SafeFlow.API.Alerts.Domain.Model.ValueObjects;

public enum IncidentResolutionFailure
{
    ActionRequired,
    AlreadyResolved
}

public sealed class IncidentResolutionException(IncidentResolutionFailure failure) : Exception(
    failure == IncidentResolutionFailure.ActionRequired
        ? "Describe la acción correctiva para resolver la incidencia."
        : "La incidencia ya fue resuelta.")
{
    public IncidentResolutionFailure Failure { get; } = failure;
}
