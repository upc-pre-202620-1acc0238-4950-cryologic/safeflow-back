using SafeFlow.API.Alerts.Application.Errors;
using SafeFlow.API.Alerts.Application.Services;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.Commands;
using SafeFlow.API.Alerts.Domain.Model.ValueObjects;
using SafeFlow.API.Alerts.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Alerts.Application.Internal.CommandServices;

public sealed class IncidentCommandService(IIncidentRepository repository, IUnitOfWork unitOfWork)
    : IIncidentCommandService
{
    public async Task<Result<Incident, IncidentError>> HandleAsync(ResolveIncidentCommand command,
        CancellationToken ct = default)
    {
        // Match the mock's validation order, including requests for an unknown incident.
        if (string.IsNullOrWhiteSpace(command.Action))
            return new Result<Incident, IncidentError>.Failure(new IncidentError.ActionRequired());

        var incident = await repository.FindByIdAsync(command.IncidentId, ct);
        if (incident is null)
            return new Result<Incident, IncidentError>.Failure(new IncidentError.NotFound());

        try
        {
            incident.Resolve(command.Action, command.Author, DateTimeOffset.UtcNow);
            await unitOfWork.CompleteAsync(ct);
            return new Result<Incident, IncidentError>.Success(incident);
        }
        catch (IncidentResolutionException exception)
        {
            return new Result<Incident, IncidentError>.Failure(IncidentError.From(exception));
        }
    }
}
