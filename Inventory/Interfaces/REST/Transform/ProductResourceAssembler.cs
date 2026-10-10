using SafeFlow.API.Inventory.Application.Services;
using SafeFlow.API.Inventory.Domain.Model.Aggregates;
using SafeFlow.API.Inventory.Domain.Model.Commands;
using SafeFlow.API.Inventory.Domain.Model.ValueObjects;
using SafeFlow.API.Inventory.Interfaces.REST.Resources;
using SafeFlow.API.Shared.Domain.Model;

namespace SafeFlow.API.Inventory.Interfaces.REST.Transform;

public static class ProductResourceAssembler
{
    public static CreateProductCommand ToCommand(CreateProductResource resource) =>
        new(resource.Name, resource.Type, resource.Minimum, resource.Maximum);

    public static ProductResource ToResource(ProductInventory inventory)
    {
        var totals = inventory.Totals;
        var product = totals.Product;
        return new ProductResource(product.Id, product.Name, product.Type, product.Minimum, product.Maximum,
            totals.Stock, totals.Reserved, totals.Available, inventory.Condition);
    }

    public static ProductResource ToResource(Product product) =>
        ToResource(new ProductInventory(new ProductStock(product, 0, 0), ThermalCondition.UNKNOWN));
}
