using SafeFlow.API.Inventory.Domain.Model.Commands;
using SafeFlow.API.Inventory.Domain.Model.ValueObjects;

namespace SafeFlow.API.Inventory.Domain.Model.Aggregates;

public sealed class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Type { get; private set; }
    public double Minimum { get; private set; }
    public double Maximum { get; private set; }

    private Product() { }

    public static Product Create(CreateProductCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Completa el nombre del producto.");
        var range = new TemperatureRange(command.Minimum, command.Maximum);
        var type = command.Type?.Trim();
        return new Product
        {
            Name = command.Name.Trim(),
            Type = string.IsNullOrEmpty(type) ? null : type,
            Minimum = range.Minimum,
            Maximum = range.Maximum
        };
    }
}
