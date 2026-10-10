using SafeFlow.API.Reporting.Domain.Model.ValueObjects;
using SafeFlow.API.Reporting.Interfaces.REST.Resources;

namespace SafeFlow.API.Reporting.Interfaces.REST.Transform;

public static class SummaryResourceAssembler
{
    public static SummaryResource ToResourceFromEntity(Summary summary) => new(
        summary.Products,
        summary.Units,
        summary.Risks,
        summary.ActiveIncidents,
        summary.InTransit,
        summary.Critical,
        summary.Warnings,
        summary.Shipments,
        summary.Preparation,
        summary.Delivered,
        summary.Empty);
}
