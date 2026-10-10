namespace SafeFlow.API.Reporting.Domain.Model.ValueObjects;

public sealed record Summary(
    int Products,
    long Units,
    int Risks,
    int ActiveIncidents,
    int InTransit,
    int Critical,
    int Warnings,
    int Shipments,
    int Preparation,
    int Delivered)
{
    public bool Empty => Products == 0 && Units == 0 && Risks == 0 && ActiveIncidents == 0
        && InTransit == 0 && Critical == 0 && Warnings == 0 && Shipments == 0
        && Preparation == 0 && Delivered == 0;
}
