using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Monitoring.Application.Errors;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Transform;

public static class MonitoringErrorResourceAssembler
{
    public static ObjectResult ToResponse(MonitoringError error) => error switch
    {
        MonitoringError.InvalidIdentifier => new BadRequestObjectResult(
            new ErrorResource("invalid_identifier", "Los identificadores deben ser enteros positivos.")),
        MonitoringError.InvalidPeriod => new UnprocessableEntityObjectResult(
            new ErrorResource("invalid_period", "La fecha inicial no puede superar la final.")),
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, "Error de Monitoring desconocido.")
    };
}
