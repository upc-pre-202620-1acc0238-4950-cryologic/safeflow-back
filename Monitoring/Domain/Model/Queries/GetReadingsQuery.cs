namespace SafeFlow.API.Monitoring.Domain.Model.Queries;

public sealed record GetReadingsQuery(int? ProductId = null, int? SensorId = null, int? ShipmentId = null,
    DateTimeOffset? From = null, DateTimeOffset? To = null);
