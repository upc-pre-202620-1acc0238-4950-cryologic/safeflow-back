namespace SafeFlow.API.Inventory.Domain.Model.ValueObjects;

public sealed record TemperatureRange
{
    public double Minimum { get; }
    public double Maximum { get; }

    public TemperatureRange(double? minimum, double? maximum)
    {
        if (minimum is not { } min || maximum is not { } max ||
            !double.IsFinite(min) || !double.IsFinite(max))
            throw new ArgumentException("Ingresa temperaturas numéricas válidas.");
        if (min > max)
            throw new ArgumentException("La temperatura mínima no puede superar a la máxima.");
        Minimum = min;
        Maximum = max;
    }
}
