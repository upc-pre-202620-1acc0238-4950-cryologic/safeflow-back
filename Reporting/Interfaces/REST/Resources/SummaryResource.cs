namespace SafeFlow.API.Reporting.Interfaces.REST.Resources;

public sealed record SummaryResource(
    int Products,
    long Units,
    int Risks,
    int ActiveIncidents,
    int InTransit,
    int Critical,
    int Warnings,
    int Shipments,
    int Preparation,
    int Delivered,
    bool Empty);
