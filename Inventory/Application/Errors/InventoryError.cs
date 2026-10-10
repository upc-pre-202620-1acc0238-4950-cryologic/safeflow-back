namespace SafeFlow.API.Inventory.Application.Errors;

public sealed record InventoryError(string Code, string Message, int? ExistingLotId = null)
{
    public static InventoryError InvalidProduct(string message) => new("invalid_product", message);
    public static InventoryError InvalidLot(string message) => new("invalid_lot", message);
    public static InventoryError InvalidIncome(string message) => new("invalid_income", message);
    public static InventoryError ProductNotFound => new("product_not_found", "Selecciona un producto existente.");
    public static InventoryError LotNotFound => new("lot_not_found", "Selecciona un lote existente.");
    public static InventoryError MissingLotProduct => new("product_not_found", "El producto del lote no existe.");
    public static InventoryError DuplicateLot(string code, int id) => new("lot_already_exists",
        $"El lote {code} ya existe. Consulta el lote registrado.", id);
    public static InventoryError ConcurrentLotChange => new("lot_concurrency_conflict",
        "El lote fue modificado por otra operación. Consulta las cantidades actualizadas e intenta nuevamente.");
    public static InventoryError InvalidCondition => new("invalid_condition",
        "El estado debe ser normal o risk.");
}
