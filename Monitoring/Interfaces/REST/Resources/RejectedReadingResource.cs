namespace SafeFlow.API.Monitoring.Interfaces.REST.Resources;

public sealed record RejectedReadingResource(int Id, int SensorId, DateTimeOffset RecordedAt, string Raw, string Reason);
