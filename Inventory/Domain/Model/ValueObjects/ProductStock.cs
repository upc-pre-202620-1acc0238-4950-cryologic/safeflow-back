using SafeFlow.API.Inventory.Domain.Model.Aggregates;

namespace SafeFlow.API.Inventory.Domain.Model.ValueObjects;

public sealed record ProductStock(Product Product, long Stock, long Reserved)
{
    public long Available => Stock - Reserved;
}
