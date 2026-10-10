using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Reporting.Application.Services;
using SafeFlow.API.Reporting.Domain.Model.Queries;
using SafeFlow.API.Reporting.Interfaces.REST.Resources;
using SafeFlow.API.Reporting.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Reporting.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/operational-summary")]
public sealed class ReportingController(IReportingQueryService queryService) : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permission.SUMMARY)]
    [SwaggerOperation(Summary = "Consulta el resumen operativo.", Tags = new[] { "Reporting" })]
    [ProducesResponseType(typeof(SummaryResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SummaryResource>> GetSummary(CancellationToken ct)
    {
        var summary = await queryService.HandleAsync(new GetSummaryQuery(), ct);
        return Ok(SummaryResourceAssembler.ToResourceFromEntity(summary));
    }
}
