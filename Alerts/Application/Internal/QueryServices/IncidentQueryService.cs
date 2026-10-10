using SafeFlow.API.Alerts.Application.Errors;
using SafeFlow.API.Alerts.Application.Services;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.Queries;
using SafeFlow.API.Alerts.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Alerts.Application.Internal.QueryServices;

public sealed class IncidentQueryService(IIncidentRepository repository) : IIncidentQueryService
{
    public async Task<Result<IReadOnlyList<Incident>, IncidentError>> HandleAsync(GetIncidentsQuery query,
        CancellationToken ct = default)
    {
        if (query.Severity is { } severity && !Enum.IsDefined(severity))
            return new Result<IReadOnlyList<Incident>, IncidentError>.Failure(new IncidentError.InvalidSeverity());

        var incidents = await repository.FindAllAsync(query.Active, query.Severity, ct);
        return new Result<IReadOnlyList<Incident>, IncidentError>.Success(incidents);
    }

    public async Task<Result<Incident, IncidentError>> HandleAsync(GetIncidentByIdQuery query,
        CancellationToken ct = default)
    {
        var incident = await repository.FindByIdAsync(query.IncidentId, ct);
        return incident is null
            ? new Result<Incident, IncidentError>.Failure(new IncidentError.NotFound())
            : new Result<Incident, IncidentError>.Success(incident);
    }
}
