using SafeFlow.API.Inventory.Domain.Model.Commands;

namespace SafeFlow.API.Inventory.Domain.Model.Aggregates;

public sealed class Lot
{
    public const int MaximumCodeLength = 255;

    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public int Reserved { get; private set; }
    public int Available => Stock - Reserved;
    public Guid Version { get; private set; }

    private Lot() { }

    public static Lot Create(CreateLotCommand command)
    {
        if (command.ProductId <= 0)
            throw new ArgumentException("Selecciona un producto existente.");
        if (string.IsNullOrWhiteSpace(command.Code))
            throw new ArgumentException("Ingresa el código del lote.");
        var code = command.Code.Trim();
        if (code.Length > MaximumCodeLength)
            throw new ArgumentException($"El código del lote no puede superar {MaximumCodeLength} caracteres.");
        return new Lot { ProductId = command.ProductId, Code = code, Version = Guid.NewGuid() };
    }

    public void AddIncome(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser un entero mayor que cero.");
        if ((long)Stock + quantity > int.MaxValue)
            throw new OverflowException("La cantidad supera el límite admitido.");
        Stock += quantity;
        Version = Guid.NewGuid();
    }

    public bool Reserve(int quantity)
    {
        if (quantity <= 0 || quantity > Available) return false;
        Reserved += quantity;
        Version = Guid.NewGuid();
        return true;
    }

    public bool Consume(int quantity)
    {
        if (quantity <= 0 || quantity > Reserved || quantity > Stock) return false;
        Stock -= quantity;
        Reserved -= quantity;
        Version = Guid.NewGuid();
        return true;
    }
}
