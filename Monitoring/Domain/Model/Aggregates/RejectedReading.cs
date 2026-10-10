namespace SafeFlow.API.Monitoring.Domain.Model.Aggregates;

public sealed class RejectedReading
{
    public int Id { get; private set; }
    public int SensorId { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string Raw { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;

    private RejectedReading() { }

    public static RejectedReading Create(int sensorId, DateTimeOffset recordedAt, string raw, string reason)
    {
        if (sensorId <= 0)
            throw new ArgumentException("El sensor no está registrado.", nameof(sensorId));
        if (recordedAt <= DateTimeOffset.UnixEpoch || recordedAt > DateTimeOffset.UtcNow)
            throw new ArgumentException("Medición o fecha inválida", nameof(recordedAt));
        ArgumentNullException.ThrowIfNull(raw);
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("El motivo del rechazo es obligatorio.", nameof(reason));

        return new RejectedReading
        {
            SensorId = sensorId,
            RecordedAt = recordedAt.ToUniversalTime(),
            Raw = raw,
            Reason = reason.Trim()
        };
    }
}
