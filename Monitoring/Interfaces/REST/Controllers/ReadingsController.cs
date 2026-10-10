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
[Route("api/v1/readings")]
[MonitoringQueryValidation]
[RequirePermission(Permission.READ_INVENTORY, Permission.WRITE_INVENTORY, Permission.CREATE_SHIPMENT,
    Permission.READ_SHIPMENTS, Permission.DELIVER, Permission.RISKS)]
public sealed class ReadingsController(IMonitoringQueryService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista las lecturas térmicas.", Tags = new[] { "Monitoring" })]
    [ProducesResponseType(typeof(ReadingResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<IReadOnlyList<ReadingResource>>> GetReadings([FromQuery] int? productId,
        [FromQuery] int? sensorId, [FromQuery] int? shipmentId, [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to, CancellationToken ct)
    {
        var result = await service.HandleAsync(new GetReadingsQuery(productId, sensorId, shipmentId, from, to), ct);
        return result.Fold<ActionResult<IReadOnlyList<ReadingResource>>>(
            readings => Ok(readings.Select(ReadingResourceAssembler.ToResource).ToList()),
            error => MonitoringErrorResourceAssembler.ToResponse(error));
    }
}
