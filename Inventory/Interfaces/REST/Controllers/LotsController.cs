using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Inventory.Application.Services;
using SafeFlow.API.Inventory.Domain.Model.Queries;
using SafeFlow.API.Inventory.Interfaces.REST.Resources;
using SafeFlow.API.Inventory.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Inventory.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/lots")]
[ServiceFilter(typeof(InventoryRequestValidationFilter), Order = -3000)]
public sealed class LotsController(IInventoryCommandService commands, IInventoryQueryService queries)
    : ControllerBase
{
    [HttpGet("{id:int}")]
    [RequirePermission(Permission.READ_INVENTORY, Permission.WRITE_INVENTORY, Permission.CREATE_SHIPMENT)]
    [SwaggerOperation(Summary = "Consulta un lote.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(LotResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLot(int id, CancellationToken ct)
    {
        var result = await queries.HandleAsync(new GetLotByIdQuery(id), ct);
        return result.Fold<IActionResult>(lot => Ok(LotResourceAssembler.ToResource(lot)),
            InventoryErrorAssembler.ToActionResult);
    }

    [HttpPost("{id:int}/incomes")]
    [RequirePermission(Permission.WRITE_INVENTORY)]
    [SwaggerOperation(Summary = "Registra un ingreso al lote.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(IncomeResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordIncome(int id, [FromBody] RecordIncomeResource resource,
        CancellationToken ct)
    {
        var command = IncomeResourceAssembler.ToCommand(id, resource, HttpContext.GetCurrentUser().Email);
        var result = await commands.HandleAsync(command, ct);
        return result.Fold<IActionResult>(income => StatusCode(StatusCodes.Status201Created,
            IncomeResourceAssembler.ToResource(income)), InventoryErrorAssembler.ToActionResult);
    }
}
