using SafeFlow.API.Alerts.Application.Internal.CommandServices;
using SafeFlow.API.Alerts.Application.Internal.QueryServices;
using SafeFlow.API.Alerts.Application.Services;
using SafeFlow.API.Alerts.Domain.Repositories;
using SafeFlow.API.Alerts.Infrastructure.Persistence.EFC.Repositories;
using SafeFlow.API.Alerts.Interfaces.ACL;
using SafeFlow.API.Alerts.Interfaces.REST.Controllers;

namespace SafeFlow.API.Alerts.Infrastructure.Configuration;

/// <summary>Registers the Alerts bounded context (repositories, services and ACL facade).</summary>
public static class AlertsContextExtensions
{
    public static IServiceCollection AddAlertsContext(this IServiceCollection services)
    {
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<IncidentUnitOfWork>();
        services.AddScoped<IIncidentCommandService>(provider => new IncidentCommandService(
            provider.GetRequiredService<IIncidentRepository>(),
            provider.GetRequiredService<IncidentUnitOfWork>()));
        services.AddScoped<IIncidentQueryService, IncidentQueryService>();
        services.AddScoped<IAlertsContextFacade, AlertsContextFacade>();
        services.AddScoped<IncidentRequestValidationFilter>();
        return services;
    }
}
