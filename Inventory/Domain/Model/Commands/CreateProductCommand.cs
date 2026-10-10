namespace SafeFlow.API.Inventory.Domain.Model.Commands;

public sealed record CreateProductCommand(string? Name, string? Type, double? Minimum, double? Maximum);
