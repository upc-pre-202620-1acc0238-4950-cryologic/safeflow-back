namespace SafeFlow.API.Monitoring.Domain.Model.Aggregates;

public sealed class Sensor
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public bool Connected { get; private set; }

    private Sensor() { }

    public static Sensor Create(int productId, string location, bool connected = true)
    {
        if (productId <= 0)
            throw new ArgumentException("El producto del sensor no es válido.", nameof(productId));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("La ubicación del sensor es obligatoria.", nameof(location));

        return new Sensor { ProductId = productId, Location = location.Trim(), Connected = connected };
    }

    public void Connect() => Connected = true;
    public void Disconnect() => Connected = false;
}
