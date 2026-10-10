using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Logistics.Application.Errors;
using SafeFlow.API.Logistics.Application.Services;
using SafeFlow.API.Logistics.Domain.Model.Commands;
using SafeFlow.API.Logistics.Domain.Model.Queries;
using SafeFlow.API.Logistics.Domain.Model.ValueObjects;
using SafeFlow.API.Logistics.Interfaces.REST.Resources;
using SafeFlow.API.Logistics.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Logistics.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/shipments")]
[ShipmentRequestValidation]
public sealed class ShipmentsController(IShipmentCommandService commands, IShipmentQueryService queries)
    : ControllerBase
{
    [HttpPost]
    [RequirePermission(Permission.CREATE_SHIPMENT)]
    [SwaggerOperation(Summary = "Crea un despacho con reserva de stock.", Tags = new[] { "Logistics" })]
    [ProducesResponseType(typeof(ShipmentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateShipmentResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(ShipmentAssembler.ToCommand(resource, HttpContext.GetCurrentUser().Email), ct);
        return result.Fold<IActionResult>(shipment => CreatedAtAction(nameof(GetById), new { id = shipment.Id },
            ShipmentAssembler.ToResource(shipment)), ToError);
    }

    [HttpGet]
    [RequirePermission(Permission.READ_SHIPMENTS, Permission.DELIVER, Permission.CREATE_SHIPMENT)]
    [SwaggerOperation(Summary = "Lista los despachos.", Tags = new[] { "Logistics" })]
    [ProducesResponseType(typeof(ShipmentResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] string? status, CancellationToken ct)
    {
        ShipmentStatus? filter = null;
        if (status is not null)
        {
            if (!Enum.TryParse<ShipmentStatus>(status, true, out var parsed) ||
                !Enum.GetNames<ShipmentStatus>().Contains(status, StringComparer.OrdinalIgnoreCase))
                return BadRequest(new ErrorResource("invalid_shipment_status", "El estado del despacho no es válido."));
            filter = parsed;
        }
        var result = await queries.Handle(new GetShipmentsQuery(filter), ct);
        return result.Fold<IActionResult>(shipments => Ok(shipments.Select(ShipmentAssembler.ToResource)), ToError);
    }

    [HttpGet("{id:int}")]
    [RequirePermission(Permission.READ_SHIPMENTS, Permission.DELIVER, Permission.CREATE_SHIPMENT)]
    [SwaggerOperation(Summary = "Consulta un despacho.", Tags = new[] { "Logistics" })]
    [ProducesResponseType(typeof(ShipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await queries.Handle(new GetShipmentByIdQuery(id), ct);
        return result.Fold<IActionResult>(shipment => Ok(ShipmentAssembler.ToResource(shipment)), ToError);
    }

    [HttpGet("{id:int}/validation")]
    [RequirePermission(Permission.READ_SHIPMENTS)]
    [SwaggerOperation(Summary = "Consulta la validación térmica del despacho.", Tags = new[] { "Logistics" })]
    [ProducesResponseType(typeof(ShipmentValidationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetValidation(int id, CancellationToken ct)
    {
        var result = await queries.Handle(new GetShipmentValidationQuery(id), ct);
        return result.Fold<IActionResult>(validation => Ok(ShipmentAssembler.ToResource(validation)), ToError);
    }

    [HttpPost("{id:int}/departure")]
    [RequirePermission(Permission.READ_SHIPMENTS)]
    [SwaggerOperation(Summary = "Registra la salida del despacho.", Tags = new[] { "Logistics" })]
    [ProducesResponseType(typeof(ShipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Depart(int id, [FromBody] DepartureResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(ShipmentAssembler.ToCommand(id, resource, HttpContext.GetCurrentUser().Email), ct);
        return result.Fold<IActionResult>(shipment => Ok(ShipmentAssembler.ToResource(shipment)), ToError);
    }

    [HttpPost("{id:int}/delivery")]
    [RequirePermission(Permission.DELIVER)]
    [SwaggerOperation(Summary = "Registra la entrega del despacho.", Tags = new[] { "Logistics" })]
    [ProducesResponseType(typeof(ShipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deliver(int id, CancellationToken ct)
    {
        var result = await commands.Handle(new DeliverShipmentCommand(id, HttpContext.GetCurrentUser().Email), ct);
        return result.Fold<IActionResult>(shipment => Ok(ShipmentAssembler.ToResource(shipment)), ToError);
    }

    private ObjectResult ToError(LogisticsError error) => StatusCode(error.Kind switch
    {
        LogisticsErrorKind.InvalidInput => StatusCodes.Status400BadRequest,
        LogisticsErrorKind.NotFound => StatusCodes.Status404NotFound,
        LogisticsErrorKind.Conflict => StatusCodes.Status409Conflict,
        LogisticsErrorKind.Unprocessable => StatusCodes.Status422UnprocessableEntity,
        _ => throw new ArgumentOutOfRangeException(nameof(error))
    }, new ErrorResource(error.Code, error.Message));
}
