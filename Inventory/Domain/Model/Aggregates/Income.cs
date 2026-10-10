using SafeFlow.API.Inventory.Domain.Model.Commands;

namespace SafeFlow.API.Inventory.Domain.Model.Aggregates;

public sealed class Income
{
    public int Id { get; private set; }
    public int LotId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string Author { get; private set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; private set; }

    private Income() { }

    public static Income Record(RecordIncomeCommand command, Lot lot, DateTimeOffset recordedAt)
    {
        if (command.LotId != lot.Id || lot.Id <= 0)
            throw new ArgumentException("Selecciona un lote existente.");
        if (command.Quantity is not > 0)
            throw new ArgumentException("La cantidad debe ser un entero mayor que cero.");
        if (string.IsNullOrWhiteSpace(command.Author))
            throw new ArgumentException("El ingreso debe tener un autor.");
        return new Income
        {
            LotId = lot.Id,
            ProductId = lot.ProductId,
            Quantity = command.Quantity.Value,
            Author = command.Author.Trim(),
            RecordedAt = recordedAt.ToUniversalTime()
        };
    }
}
