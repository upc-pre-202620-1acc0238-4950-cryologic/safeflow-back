using SafeFlow.API.Reporting.Application.Internal.QueryServices;
using SafeFlow.API.Reporting.Application.Services;

namespace SafeFlow.API.Reporting.Infrastructure.Configuration;

/// <summary>Registers the Reporting read-only query service.</summary>
public static class ReportingContextExtensions
{
    public static IServiceCollection AddReportingContext(this IServiceCollection services)
    {
        services.AddScoped<IReportingQueryService, ReportingQueryService>();
        return services;
    }
}
