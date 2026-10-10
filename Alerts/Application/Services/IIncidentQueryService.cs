using SafeFlow.API.Alerts.Application.Errors;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.Queries;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Alerts.Application.Services;

public interface IIncidentQueryService
{
    Task<Result<IReadOnlyList<Incident>, IncidentError>> HandleAsync(GetIncidentsQuery query,
        CancellationToken ct = default);
    Task<Result<Incident, IncidentError>> HandleAsync(GetIncidentByIdQuery query,
        CancellationToken ct = default);
}
