using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Commands;
using SafeFlow.API.Inventory.Interfaces.REST.Resources;

namespace SafeFlow.API.Inventory.Interfaces.REST.Transform;

public static class LotResourceAssembler
{
    public static CreateLotCommand ToCommand(int productId, CreateLotResource resource) =>
        new(productId, resource.Code);

    public static LotResource ToResource(Lot lot) =>
        new(lot.Id, lot.ProductId, lot.Code, lot.Stock, lot.Reserved, lot.Available);
}
