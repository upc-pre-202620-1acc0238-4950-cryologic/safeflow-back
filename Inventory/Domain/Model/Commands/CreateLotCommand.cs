namespace SafeFlow.API.Inventory.Domain.Model.Commands;

public sealed record CreateLotCommand(int ProductId, string? Code);
