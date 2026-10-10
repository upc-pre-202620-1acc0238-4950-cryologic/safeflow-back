namespace SafeFlow.API.Logistics.Domain.Model.Commands;

public sealed record DepartShipmentCommand(int ShipmentId, bool AcceptRisk, string DepartedBy);
