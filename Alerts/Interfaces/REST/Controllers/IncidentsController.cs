using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Alerts.Application.Errors;
using SafeFlow.API.Alerts.Application.Services;
using SafeFlow.API.Alerts.Domain.Model.Queries;
using SafeFlow.API.Alerts.Interfaces.REST.Resources;
using SafeFlow.API.Alerts.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Domain.Model;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Alerts.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/incidents")]
[ServiceFilter(typeof(IncidentRequestValidationFilter), Order = -2001)]
public sealed class IncidentsController(IIncidentQueryService queryService, IIncidentCommandService commandService)
    : ControllerBase
{
    [HttpGet]
    [RequirePermission(Permission.ALERTS, Permission.RESOLVE)]
    [SwaggerOperation(Summary = "Lista las incidencias.", Tags = new[] { "Alerts" })]
    [ProducesResponseType(typeof(IncidentResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] bool? active, [FromQuery] ThermalCondition? severity,
        CancellationToken ct)
    {
        var result = await queryService.HandleAsync(new GetIncidentsQuery(active, severity), ct);
        return result.Fold<IActionResult>(
            incidents => Ok(incidents.Select(IncidentAssembler.ToResource)),
            ErrorResponse);
    }

    [HttpGet("{id}")]
    [RequirePermission(Permission.ALERTS, Permission.RESOLVE)]
    [SwaggerOperation(Summary = "Consulta una incidencia.", Tags = new[] { "Alerts" })]
    [ProducesResponseType(typeof(IncidentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await queryService.HandleAsync(new GetIncidentByIdQuery(id), ct);
        return result.Fold<IActionResult>(incident => Ok(IncidentAssembler.ToResource(incident)), ErrorResponse);
    }

    [HttpPost("{id}/resolution")]
    [RequirePermission(Permission.RESOLVE)]
    [SwaggerOperation(Summary = "Registra la resolución de una incidencia.", Tags = new[] { "Alerts" })]
    [ProducesResponseType(typeof(IncidentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Resolve(int id, [FromBody] ResolveIncidentResource resource,
        CancellationToken ct)
    {
        var command = IncidentAssembler.ToCommand(id, resource, HttpContext.GetCurrentUser().Email);
        var result = await commandService.HandleAsync(command, ct);
        return result.Fold<IActionResult>(incident => Ok(IncidentAssembler.ToResource(incident)), ErrorResponse);
    }

    private IActionResult ErrorResponse(IncidentError error)
    {
        var resource = new ErrorResource(error.Code, error.Message);
        return error switch
        {
            IncidentError.NotFound => NotFound(resource),
            IncidentError.AlreadyResolved => Conflict(resource),
            IncidentError.ActionRequired => UnprocessableEntity(resource),
            _ => BadRequest(resource)
        };
    }
}
