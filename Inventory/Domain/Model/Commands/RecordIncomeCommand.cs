namespace SafeFlow.API.Inventory.Domain.Model.Commands;

public sealed record RecordIncomeCommand(int LotId, int? Quantity, string Author);
