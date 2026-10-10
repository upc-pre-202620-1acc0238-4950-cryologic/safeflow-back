using SafeFlow.API.Monitoring.Application.Errors;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Model.Queries;
using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Monitoring.Application.Services;

public interface IMonitoringQueryService
{
    Task<Result<IReadOnlyList<SensorStatus>, MonitoringError>> HandleAsync(GetSensorsQuery query, CancellationToken ct = default);
    Task<Result<IReadOnlyList<Reading>, MonitoringError>> HandleAsync(GetReadingsQuery query, CancellationToken ct = default);
    Task<Result<IReadOnlyList<RejectedReading>, MonitoringError>> HandleAsync(GetRejectionsQuery query, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ProductThermalStatus>, MonitoringError>> HandleAsync(GetRisksQuery query, CancellationToken ct = default);
}
