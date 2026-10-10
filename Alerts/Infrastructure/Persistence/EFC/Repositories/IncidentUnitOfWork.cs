using Microsoft.EntityFrameworkCore;
using SafeFlow.API.Alerts.Domain.Model.Aggregates;
using SafeFlow.API.Alerts.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Alerts.Infrastructure.Persistence.EFC.Repositories;

/// <summary>Translates persistence conflicts without exposing EF exceptions to Application.</summary>
public sealed class IncidentUnitOfWork(IUnitOfWork unitOfWork) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
            when (exception.Entries.Count > 0 && exception.Entries.All(entry => entry.Entity is Incident))
        {
            throw new IncidentResolutionException(IncidentResolutionFailure.AlreadyResolved);
        }
    }
}
