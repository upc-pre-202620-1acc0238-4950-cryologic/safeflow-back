using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Configuration;

public sealed class BoundedContextSwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        document.Tags =
        [
            new OpenApiTag { Name = "Iam", Description = "Autenticación, recuperación, perfil y permisos." },
            new OpenApiTag { Name = "Inventory", Description = "Productos, lotes e ingresos de inventario." },
            new OpenApiTag { Name = "Monitoring", Description = "Sensores, lecturas y riesgos térmicos." },
            new OpenApiTag { Name = "Alerts", Description = "Incidencias y acciones correctivas." },
            new OpenApiTag { Name = "Logistics", Description = "Despachos, validación, salida y entrega." },
            new OpenApiTag { Name = "Reporting", Description = "Resumen operativo consolidado." }
        ];
    }
}
