using SafeFlow.API.Reporting.Domain.Model.Queries;
using SafeFlow.API.Reporting.Domain.Model.ValueObjects;

namespace SafeFlow.API.Reporting.Application.Services;

public interface IReportingQueryService
{
    Task<Summary> HandleAsync(GetSummaryQuery query, CancellationToken ct = default);
}
