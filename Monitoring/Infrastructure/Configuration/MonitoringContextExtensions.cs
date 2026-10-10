using SafeFlow.API.Monitoring.Application.Internal.QueryServices;
using SafeFlow.API.Monitoring.Application.Services;
using SafeFlow.API.Monitoring.Domain.Repositories;
using SafeFlow.API.Monitoring.Infrastructure.Persistence.EFC.Repositories;
using SafeFlow.API.Monitoring.Interfaces.ACL;

namespace SafeFlow.API.Monitoring.Infrastructure.Configuration;

/// <summary>Registers the Monitoring bounded context (repositories, services and ACL facade).</summary>
public static class MonitoringContextExtensions
{
    public static IServiceCollection AddMonitoringContext(this IServiceCollection services)
    {
        services.AddScoped<ISensorRepository, SensorRepository>();
        services.AddScoped<IReadingRepository, ReadingRepository>();
        services.AddScoped<IRejectedReadingRepository, RejectedReadingRepository>();
        services.AddScoped<IMonitoringQueryService, MonitoringQueryService>();
        services.AddScoped<IMonitoringContextFacade, MonitoringContextFacade>();
        return services;
    }
}
