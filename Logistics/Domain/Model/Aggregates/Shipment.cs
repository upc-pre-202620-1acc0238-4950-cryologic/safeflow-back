using SafeFlow.API.Logistics.Domain.Model.Commands;
using SafeFlow.API.Logistics.Domain.Model.ValueObjects;

namespace SafeFlow.API.Logistics.Domain.Model.Aggregates;

public sealed class Shipment
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int LotId { get; private set; }
    public int SensorId { get; private set; }
    public int Quantity { get; private set; }
    public string Destination { get; private set; } = string.Empty;
    public ShipmentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset? DepartedAt { get; private set; }
    public string? DepartedBy { get; private set; }
    public bool RiskAccepted { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public string? DeliveredBy { get; private set; }

    private Shipment() { }

    public static Shipment Create(CreateShipmentCommand command, DateTimeOffset at)
    {
        if (string.IsNullOrWhiteSpace(command.Destination))
            throw new ArgumentException("Ingresa el destino del despacho.");
        if (command.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser un entero mayor que cero.");
        if (command.ProductId <= 0 || command.LotId <= 0 || command.SensorId <= 0)
            throw new ArgumentException("El producto, lote y sensor deben existir.");
        ArgumentException.ThrowIfNullOrWhiteSpace(command.CreatedBy);

        return new Shipment
        {
            ProductId = command.ProductId,
            LotId = command.LotId,
            SensorId = command.SensorId,
            Quantity = command.Quantity,
            Destination = command.Destination.Trim(),
            Status = ShipmentStatus.PREPARATION,
            CreatedAt = at.ToUniversalTime(),
            CreatedBy = command.CreatedBy
        };
    }

    public void Depart(bool riskAccepted, string by, DateTimeOffset at)
    {
        if (Status != ShipmentStatus.PREPARATION)
            throw new InvalidOperationException("El despacho ya fue enviado.");
        ArgumentException.ThrowIfNullOrWhiteSpace(by);
        Status = ShipmentStatus.TRANSIT;
        DepartedAt = at.ToUniversalTime();
        DepartedBy = by;
        RiskAccepted = riskAccepted;
    }

    public void Deliver(string by, DateTimeOffset at)
    {
        if (Status == ShipmentStatus.DELIVERED)
            throw new InvalidOperationException("El despacho ya fue entregado. Se conserva su fecha original.");
        if (Status != ShipmentStatus.TRANSIT)
            throw new InvalidOperationException("Solo se puede entregar un despacho en tránsito.");
        ArgumentException.ThrowIfNullOrWhiteSpace(by);
        Status = ShipmentStatus.DELIVERED;
        DeliveredAt = at.ToUniversalTime();
        DeliveredBy = by;
    }
}
