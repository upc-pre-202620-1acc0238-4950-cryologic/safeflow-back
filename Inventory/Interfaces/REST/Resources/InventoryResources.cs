using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Inventory.Interfaces.REST.Resources;

public sealed record CreateProductResource(string? Name, string? Type, double? Minimum, double? Maximum);
public sealed record CreateLotResource(string? Code);
public sealed record RecordIncomeResource(int? Quantity);
public sealed record ProductResource(int Id, string Name, string? Type, double Minimum, double Maximum,
    long Stock, long Reserved, long Available, ThermalCondition Condition);
public sealed record LotResource(int Id, int ProductId, string Code, int Stock, int Reserved, int Available);
public sealed record IncomeResource(int Id, int LotId, int ProductId, int Quantity, string Author,
    DateTimeOffset RecordedAt);
public sealed record DuplicateLotErrorResource(string Error, string Message, int ExistingLotId);
