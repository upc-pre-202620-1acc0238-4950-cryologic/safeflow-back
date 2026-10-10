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
[Route("api/v1/rejected-readings")]
[MonitoringQueryValidation]
[RequirePermission(Permission.RISKS, Permission.READ_INVENTORY)]
public sealed class RejectionsController(IMonitoringQueryService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista las lecturas rechazadas.", Tags = new[] { "Monitoring" })]
    [ProducesResponseType(typeof(RejectedReadingResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<RejectedReadingResource>>> GetRejections([FromQuery] int? sensorId,
        CancellationToken ct)
    {
        var result = await service.HandleAsync(new GetRejectionsQuery(sensorId), ct);
        return result.Fold<ActionResult<IReadOnlyList<RejectedReadingResource>>>(
            readings => Ok(readings.Select(RejectedReadingResourceAssembler.ToResource).ToList()),
            error => MonitoringErrorResourceAssembler.ToResponse(error));
    }
}
