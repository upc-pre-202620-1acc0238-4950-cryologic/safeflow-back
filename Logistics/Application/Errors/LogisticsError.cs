namespace SafeFlow.API.Logistics.Application.Errors;

public enum LogisticsErrorKind
{
    InvalidInput,
    NotFound,
    Conflict,
    Unprocessable
}

public sealed record LogisticsError(string Code, string Message, LogisticsErrorKind Kind)
{
    public static readonly LogisticsError DestinationRequired = new("destination_required",
        "Ingresa el destino del despacho.", LogisticsErrorKind.InvalidInput);
    public static readonly LogisticsError InvalidQuantity = new("invalid_quantity",
        "La cantidad debe ser un entero mayor que cero.", LogisticsErrorKind.InvalidInput);
    public static readonly LogisticsError SelectedProductNotFound = new("product_not_found",
        "Selecciona un producto existente.", LogisticsErrorKind.NotFound);
    public static readonly LogisticsError SelectedLotNotFound = new("lot_not_found",
        "Selecciona un lote existente.", LogisticsErrorKind.NotFound);
    public static readonly LogisticsError LotProductMismatch = new("lot_product_mismatch",
        "El lote no pertenece al producto seleccionado.", LogisticsErrorKind.InvalidInput);
    public static readonly LogisticsError SensorNotFound = new("sensor_not_found",
        "Selecciona un sensor registrado. La integración de sensores está pendiente.", LogisticsErrorKind.NotFound);
    public static readonly LogisticsError SensorProductMismatch = new("sensor_product_mismatch",
        "El sensor no corresponde al producto.", LogisticsErrorKind.InvalidInput);
    public static readonly LogisticsError SensorInUse = new("sensor_in_use",
        "El sensor está asociado a otro despacho activo.", LogisticsErrorKind.Conflict);
    public static readonly LogisticsError ShipmentNotFound = new("shipment_not_found",
        "El despacho no existe.", LogisticsErrorKind.NotFound);
    public static readonly LogisticsError AlreadyDeparted = new("shipment_already_departed",
        "El despacho ya fue enviado.", LogisticsErrorKind.Conflict);
    public static readonly LogisticsError SensorDisconnected = new("sensor_disconnected",
        "No se puede enviar: sensor desconectado o sin temperatura actual.", LogisticsErrorKind.Unprocessable);
    public static readonly LogisticsError ReadingUnavailable = new("current_reading_unavailable",
        "No hay una temperatura actual. La lectura debe tener menos de 5 minutos.", LogisticsErrorKind.Unprocessable);
    public static readonly LogisticsError ProductNotFound = new("product_not_found",
        "El producto no existe.", LogisticsErrorKind.NotFound);
    public static readonly LogisticsError RiskConfirmationRequired = new("risk_confirmation_required",
        "El producto presenta riesgo térmico. Confirma expresamente para continuar.", LogisticsErrorKind.Unprocessable);
    public static readonly LogisticsError AlreadyDelivered = new("shipment_already_delivered",
        "El despacho ya fue entregado. Se conserva su fecha original.", LogisticsErrorKind.Conflict);
    public static readonly LogisticsError NotInTransit = new("shipment_not_in_transit",
        "Solo se puede entregar un despacho en tránsito.", LogisticsErrorKind.Conflict);
    public static readonly LogisticsError LotNotFound = new("lot_not_found",
        "El lote no existe.", LogisticsErrorKind.NotFound);
    public static readonly LogisticsError InconsistentReservation = new("inconsistent_reservation",
        "El stock reservado no es consistente.", LogisticsErrorKind.Conflict);

    public static LogisticsError InsufficientStock(int available) => new("insufficient_stock",
        $"La cantidad supera el stock disponible del lote ({available}).", LogisticsErrorKind.Conflict);
}
