using SafeFlow.API.Alerts.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Alerts.Domain.Model.Aggregates;

public sealed class Incident
{
    private Incident() { }

    public int Id { get; private set; }
    public int SensorId { get; private set; }
    public int ProductId { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public ThermalCondition Severity { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTimeOffset OpenedAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public int Repetitions { get; private set; } = 1;
    public string? Action { get; private set; }
    public string? Author { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public bool Active => ResolvedAt is null;

    public static Incident Create(int sensorId, int productId, string location, string type,
        ThermalCondition severity, string description, DateTimeOffset at)
    {
        if (sensorId <= 0) throw new ArgumentOutOfRangeException(nameof(sensorId));
        if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        ArgumentException.ThrowIfNullOrWhiteSpace(location);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (!Enum.IsDefined(severity)) throw new ArgumentOutOfRangeException(nameof(severity));

        return new Incident
        {
            SensorId = sensorId,
            ProductId = productId,
            Location = location.Trim(),
            Type = type.Trim(),
            Severity = severity,
            Description = description.Trim(),
            OpenedAt = at.ToUniversalTime(),
            LastSeenAt = at.ToUniversalTime()
        };
    }

    public void Resolve(string? action, string author, DateTimeOffset at)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new IncidentResolutionException(IncidentResolutionFailure.ActionRequired);
        if (!Active)
            throw new IncidentResolutionException(IncidentResolutionFailure.AlreadyResolved);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        Action = action.Trim();
        Author = author;
        ResolvedAt = at.ToUniversalTime();
    }
}
