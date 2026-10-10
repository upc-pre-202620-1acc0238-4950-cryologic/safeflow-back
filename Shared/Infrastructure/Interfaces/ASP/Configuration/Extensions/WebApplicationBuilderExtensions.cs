using System.Text.Json.Serialization;
using SafeFlow.API.Alerts.Infrastructure.Configuration;
using SafeFlow.API.Iam.Infrastructure.Configuration;
using SafeFlow.API.Inventory.Infrastructure.Configuration;
using SafeFlow.API.Logistics.Infrastructure.Configuration;
using SafeFlow.API.Monitoring.Infrastructure.Configuration;
using SafeFlow.API.Reporting.Infrastructure.Configuration;
using SafeFlow.API.Shared.Domain.Repositories;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddSafeFlowServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(connectionString));

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddIamContext(builder.Configuration);
        builder.Services.AddInventoryContext();
        builder.Services.AddMonitoringContext();
        builder.Services.AddAlertsContext();
        builder.Services.AddLogisticsContext();
        builder.Services.AddReportingContext();

        builder.Services.AddControllers(options =>
                options.Conventions.Add(new KebabCaseRouteNamingConvention()))
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.DocumentFilter<BoundedContextSwaggerDocumentFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SafeFlow API",
                Version = "v1",
                Description = "API para inventario, monitoreo térmico y operaciones logísticas de SafeFlow."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT obtenido al crear una sesión, sin el prefijo Bearer."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddCors(options =>
        {
            var extraOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

            options.AddPolicy("SafeFlowFrontend", policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                    {
                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            return false;
                        if (uri.Scheme is not "http" and not "https")
                            return false;
                        if (uri.Host is "localhost" or "127.0.0.1")
                            return true;
                        if (uri.Host.EndsWith(".netlify.app", StringComparison.OrdinalIgnoreCase))
                            return true;
                        return extraOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }
}
