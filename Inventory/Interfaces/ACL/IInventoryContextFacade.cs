namespace SafeFlow.API.Inventory.Interfaces.ACL;

public sealed record ProductSnapshot(int Id, string Name, string? Type, double Minimum, double Maximum);

public sealed record LotSnapshot(int Id, int ProductId, string Code, int Stock, int Reserved)
{
    public int Available => Stock - Reserved;
}

public sealed record InventoryTotalsSnapshot(int Products, long Units);

/// <summary>
/// Open host service of Inventory for other contexts. Reserve/Consume only change tracked state;
/// the caller commits them with its own IUnitOfWork so the shipment and the lot change are atomic.
/// </summary>
public interface IInventoryContextFacade
{
    Task<ProductSnapshot?> FindProductAsync(int productId, CancellationToken ct = default);
    Task<IReadOnlyList<ProductSnapshot>> ListProductsAsync(CancellationToken ct = default);
    Task<LotSnapshot?> FindLotAsync(int lotId, CancellationToken ct = default);
    Task<InventoryTotalsSnapshot> TotalsAsync(CancellationToken ct = default);

    /// <summary>Returns false when the lot does not exist or its available stock is insufficient.</summary>
    Task<bool> ReserveAsync(int lotId, int quantity, CancellationToken ct = default);

    /// <summary>Consumes previously reserved units. Returns false when the reservation is inconsistent.</summary>
    Task<bool> ConsumeAsync(int lotId, int quantity, CancellationToken ct = default);
}
