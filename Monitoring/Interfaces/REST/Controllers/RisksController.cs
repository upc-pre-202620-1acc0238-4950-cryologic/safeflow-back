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
[Route("api/v1/risks")]
[RequirePermission(Permission.RISKS)]
public sealed class RisksController(IMonitoringQueryService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista los riesgos térmicos.", Tags = new[] { "Monitoring" })]
    [ProducesResponseType(typeof(RiskResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<RiskResource>>> GetRisks(CancellationToken ct)
    {
        var result = await service.HandleAsync(new GetRisksQuery(), ct);
        return result.Fold<ActionResult<IReadOnlyList<RiskResource>>>(
            risks => Ok(risks.Select(RiskResourceAssembler.ToResource).ToList()),
            error => MonitoringErrorResourceAssembler.ToResponse(error));
    }
}
