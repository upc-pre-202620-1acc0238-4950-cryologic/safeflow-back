using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Commands;
using SafeFlow.API.Inventory.Interfaces.REST.Resources;

namespace SafeFlow.API.Inventory.Interfaces.REST.Transform;

public static class IncomeResourceAssembler
{
    public static RecordIncomeCommand ToCommand(int lotId, RecordIncomeResource resource, string author) =>
        new(lotId, resource.Quantity, author);

    public static IncomeResource ToResource(Income income) =>
        new(income.Id, income.LotId, income.ProductId, income.Quantity, income.Author, income.RecordedAt);
}
