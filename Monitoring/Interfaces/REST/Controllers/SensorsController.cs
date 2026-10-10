using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Monitoring.Application.Services;
using SafeFlow.API.Monitoring.Domain.Model.Queries;
using SafeFlow.API.Monitoring.Interfaces.REST.Resources;
using SafeFlow.API.Monitoring.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/sensors")]
[MonitoringQueryValidation]
[RequirePermission(Permission.READ_INVENTORY, Permission.WRITE_INVENTORY, Permission.CREATE_SHIPMENT,
    Permission.READ_SHIPMENTS, Permission.DELIVER, Permission.RISKS)]
public sealed class SensorsController(IMonitoringQueryService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista los sensores.", Tags = new[] { "Monitoring" })]
    [ProducesResponseType(typeof(SensorResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<SensorResource>>> GetSensors([FromQuery] int? productId,
        CancellationToken ct)
    {
        var result = await service.HandleAsync(new GetSensorsQuery(productId), ct);
        return result.Fold<ActionResult<IReadOnlyList<SensorResource>>>(
            sensors => Ok(sensors.Select(SensorResourceAssembler.ToResource).ToList()),
            error => MonitoringErrorResourceAssembler.ToResponse(error));
    }
}
