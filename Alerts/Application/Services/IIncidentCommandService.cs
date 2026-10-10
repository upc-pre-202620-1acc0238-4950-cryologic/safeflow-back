using SafeFlow.API.Alerts.Application.Errors;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.Commands;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Alerts.Application.Services;

public interface IIncidentCommandService
{
    Task<Result<Incident, IncidentError>> HandleAsync(ResolveIncidentCommand command,
        CancellationToken ct = default);
}
