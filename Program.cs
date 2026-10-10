using System.Net.Sockets;
using SafeFlow.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;
using SafeFlow.API.Shared.Infrastructure.Persistence.EFC.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddSafeFlowServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        // 1. Ejecutamos el seeder de forma segura
        await SafeFlowDbSeeder.InitializeAsync(scope.ServiceProvider, app.Configuration);
    }
    catch (Exception ex)
    {
        // 2. Si falla la BD, capturamos el error para que la API no muera (evita el 503)
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, ">>> ERROR CRÍTICO AL CONECTAR O SEMBRAR LA BD EN AZURE <<<");
    }
}

// Permite Swagger en cualquier entorno
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SafeFlow API v1");
    c.RoutePrefix = "swagger"; // O string.Empty si quieres que sea la página principal
});

app.UseCors("SafeFlowFrontend");
app.UseRequestAuthorization();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.MapControllers();

try
{
    app.Run();
}
catch (IOException ex) when (IsPortInUse(ex))
{
    PrintPortInUseHelp();
    return 1;
}

return 0;

static bool IsPortInUse(Exception ex)
{
    for (var current = ex; current != null; current = current.InnerException)
    {
        if (current is SocketException { SocketErrorCode: SocketError.AddressAlreadyInUse })
            return true;
        if (current.Message.Contains("address already in use", StringComparison.OrdinalIgnoreCase)
            || current.Message.Contains("Solo se permite un uso de cada dirección", StringComparison.OrdinalIgnoreCase))
            return true;
    }

    return false;
}

static void PrintPortInUseHelp()
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("ERROR: El puerto 5115 ya esta en uso (otra instancia de safeflow-backend sigue activa).");
    Console.Error.WriteLine("Solucion: detén la instancia anterior (Ctrl+C en su terminal) o libera el puerto 5115.");
    Console.Error.WriteLine();
}
