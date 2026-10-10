using SafeFlow.API.Logistics.Application.Internal.CommandServices;
using SafeFlow.API.Logistics.Application.Internal.QueryServices;
using SafeFlow.API.Logistics.Application.Services;
using SafeFlow.API.Logistics.Domain.Repositories;
using SafeFlow.API.Logistics.Infrastructure.Persistence.EFC.Repositories;
using SafeFlow.API.Logistics.Interfaces.ACL;

namespace SafeFlow.API.Logistics.Infrastructure.Configuration;

/// <summary>Registers the Logistics bounded context (repositories, services and ACL facade).</summary>
public static class LogisticsContextExtensions
{
    public static IServiceCollection AddLogisticsContext(this IServiceCollection services)
    {
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IShipmentCommandService, ShipmentCommandService>();
        services.AddScoped<IShipmentQueryService, ShipmentQueryService>();
        services.AddScoped<IShipmentValidationService, ShipmentValidationService>();
        services.AddScoped<ILogisticsContextFacade, LogisticsContextFacade>();
        return services;
    }
}
