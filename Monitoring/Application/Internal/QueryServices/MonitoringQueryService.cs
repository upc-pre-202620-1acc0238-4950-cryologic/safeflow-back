using SafeFlow.API.Monitoring.Application.Errors;
using SafeFlow.API.Monitoring.Application.Services;
using SafeFlow.API.Monitoring.Domain.Model.Aggregates;
using SafeFlow.API.Monitoring.Domain.Model.Queries;
using SafeFlow.API.Monitoring.Domain.Model.ValueObjects;
using SafeFlow.API.Monitoring.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Monitoring.Application.Internal.QueryServices;

public sealed class MonitoringQueryService(ISensorRepository sensors, IReadingRepository readings,
    IRejectedReadingRepository rejections) : IMonitoringQueryService
{
    public async Task<Result<IReadOnlyList<SensorStatus>, MonitoringError>> HandleAsync(GetSensorsQuery query,
        CancellationToken ct = default)
    {
        if (query.ProductId is <= 0)
            return new Result<IReadOnlyList<SensorStatus>, MonitoringError>.Failure(MonitoringError.InvalidIdentifier);

        return new Result<IReadOnlyList<SensorStatus>, MonitoringError>.Success(
            await sensors.ListWithLatestReadingAsync(query.ProductId, ct));
    }

    public async Task<Result<IReadOnlyList<Reading>, MonitoringError>> HandleAsync(GetReadingsQuery query,
        CancellationToken ct = default)
    {
        if (query.ProductId is <= 0 || query.SensorId is <= 0 || query.ShipmentId is <= 0)
            return new Result<IReadOnlyList<Reading>, MonitoringError>.Failure(MonitoringError.InvalidIdentifier);
        if (query.From.HasValue && query.To.HasValue && query.From.Value > query.To.Value)
            return new Result<IReadOnlyList<Reading>, MonitoringError>.Failure(MonitoringError.InvalidPeriod);

        var utcQuery = query with { From = query.From?.ToUniversalTime(), To = query.To?.ToUniversalTime() };
        return new Result<IReadOnlyList<Reading>, MonitoringError>.Success(await readings.FindAsync(utcQuery, ct));
    }

    public async Task<Result<IReadOnlyList<RejectedReading>, MonitoringError>> HandleAsync(GetRejectionsQuery query,
        CancellationToken ct = default)
    {
        if (query.SensorId is <= 0)
            return new Result<IReadOnlyList<RejectedReading>, MonitoringError>.Failure(MonitoringError.InvalidIdentifier);

        return new Result<IReadOnlyList<RejectedReading>, MonitoringError>.Success(
            await rejections.FindBySensorAsync(query.SensorId, ct));
    }

    public async Task<Result<IReadOnlyList<ProductThermalStatus>, MonitoringError>> HandleAsync(GetRisksQuery query,
        CancellationToken ct = default)
    {
        var statuses = await sensors.ListWithLatestReadingAsync(null, ct);
        var risks = ProductThermalStatus.FromSensors(statuses)
            .Where(status => status.Condition is ThermalCondition.WARNING or ThermalCondition.RISK
                or ThermalCondition.CRITICAL or ThermalCondition.DISCONNECTED).ToList();
        return new Result<IReadOnlyList<ProductThermalStatus>, MonitoringError>.Success(risks);
    }
}
