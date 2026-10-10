using SafeFlow.API.Shared.Domain.Model;
using SafeFlow.API.Shared.Domain.Services;

namespace SafeFlow.API.Monitoring.Domain.Model.Aggregates;

public sealed class Reading
{
    public int Id { get; private set; }
    public int SensorId { get; private set; }
    public int? ShipmentId { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public double Temperature { get; private set; }
    public ThermalCondition Condition { get; private set; }

    private Reading() { }

    public static Reading Create(int sensorId, int? shipmentId, DateTimeOffset recordedAt,
        double temperature, double minimum, double maximum)
    {
        if (sensorId <= 0)
            throw new ArgumentException("El sensor no está registrado.", nameof(sensorId));
        if (shipmentId is <= 0)
            throw new ArgumentException("El despacho no es válido.", nameof(shipmentId));
        if (recordedAt <= DateTimeOffset.UnixEpoch || recordedAt > DateTimeOffset.UtcNow)
            throw new ArgumentException("Medición o fecha inválida", nameof(recordedAt));
        if (!double.IsFinite(temperature))
            throw new ArgumentException("Medición o fecha inválida", nameof(temperature));
        if (!double.IsFinite(minimum) || !double.IsFinite(maximum) || minimum > maximum)
            throw new ArgumentException("El rango térmico del producto no es válido.");

        return new Reading
        {
            SensorId = sensorId,
            ShipmentId = shipmentId,
            RecordedAt = recordedAt.ToUniversalTime(),
            Temperature = temperature,
            Condition = ThermalRules.Evaluate(temperature, minimum, maximum)
        };
    }
}
