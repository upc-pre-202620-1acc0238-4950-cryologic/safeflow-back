namespace SafeFlow.API.Inventory.Domain.Model.Queries;

public sealed record GetProductsQuery(string? Condition);
public sealed record GetProductByIdQuery(int ProductId);
public sealed record GetProductLotsQuery(int ProductId);
public sealed record GetLotByIdQuery(int LotId);
