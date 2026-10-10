using SafeFlow.API.Alerts.Interfaces.ACL;
using SafeFlow.API.Inventory.Interfaces.ACL;
using SafeFlow.API.Logistics.Interfaces.ACL;
using SafeFlow.API.Monitoring.Interfaces.ACL;
using SafeFlow.API.Reporting.Application.Services;
using SafeFlow.API.Reporting.Domain.Model.Queries;
using SafeFlow.API.Reporting.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Reporting.Application.Internal.QueryServices;

public sealed class ReportingQueryService(
    IInventoryContextFacade inventory,
    IMonitoringContextFacade monitoring,
    IAlertsContextFacade alerts,
    ILogisticsContextFacade logistics) : IReportingQueryService
{
    public async Task<Summary> HandleAsync(GetSummaryQuery query, CancellationToken ct = default)
    {
        // Facades can share a scoped DbContext, which does not support concurrent queries.
        var totals = await inventory.TotalsAsync(ct);
        var conditions = await monitoring.ProductConditionsAsync(ct);
        var incidents = await alerts.CountsAsync(ct);
        var shipments = await logistics.CountsAsync(ct);

        var risks = conditions.Values.Count(condition =>
            condition is ThermalCondition.WARNING or ThermalCondition.RISK or ThermalCondition.CRITICAL);

        return new Summary(
            totals.Products,
            totals.Units,
            risks,
            incidents.Active,
            shipments.Transit,
            incidents.ActiveCritical,
            incidents.ActiveWarnings,
            shipments.Total,
            shipments.Preparation,
            shipments.Delivered);
    }
}
