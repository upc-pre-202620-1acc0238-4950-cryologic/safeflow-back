using SafeFlow.API.Inventory.Application.Internal.CommandServices;
using SafeFlow.API.Inventory.Application.Internal.QueryServices;
using SafeFlow.API.Inventory.Application.Services;
using SafeFlow.API.Inventory.Domain.Repositories;
using SafeFlow.API.Inventory.Infrastructure.Persistence.EFC.Repositories;
using SafeFlow.API.Inventory.Interfaces.ACL;
using SafeFlow.API.Inventory.Interfaces.REST.Controllers;

namespace SafeFlow.API.Inventory.Infrastructure.Configuration;

/// <summary>Registers the Inventory bounded context (repositories, services and ACL facade).</summary>
public static class InventoryContextExtensions
{
    public static IServiceCollection AddInventoryContext(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ILotRepository, LotRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IInventoryCommandService, InventoryCommandService>();
        services.AddScoped<IInventoryQueryService, InventoryQueryService>();
        services.AddScoped<IInventoryContextFacade, InventoryContextFacade>();
        services.AddScoped<InventoryRequestValidationFilter>();
        return services;
    }
}
