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
[Route("api/v1/products")]
[ServiceFilter(typeof(InventoryRequestValidationFilter), Order = -3000)]
public sealed class ProductsController(IInventoryCommandService commands, IInventoryQueryService queries)
    : ControllerBase
{
    [HttpPost]
    [RequirePermission(Permission.WRITE_INVENTORY)]
    [SwaggerOperation(Summary = "Crea un producto.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductResource resource, CancellationToken ct)
    {
        var result = await commands.HandleAsync(ProductResourceAssembler.ToCommand(resource), ct);
        return result.Fold<IActionResult>(product => CreatedAtAction(nameof(GetProduct), new { id = product.Id },
            ProductResourceAssembler.ToResource(product)), InventoryErrorAssembler.ToActionResult);
    }

    [HttpGet]
    [RequirePermission(Permission.READ_INVENTORY, Permission.WRITE_INVENTORY, Permission.CREATE_SHIPMENT,
        Permission.RISKS)]
    [SwaggerOperation(Summary = "Lista los productos.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(ProductResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProducts([FromQuery] string? condition, CancellationToken ct)
    {
        var result = await queries.HandleAsync(new GetProductsQuery(condition), ct);
        return result.Fold<IActionResult>(products => Ok(products.Select(ProductResourceAssembler.ToResource)),
            InventoryErrorAssembler.ToActionResult);
    }

    [HttpGet("{id:int}")]
    [RequirePermission(Permission.READ_INVENTORY, Permission.WRITE_INVENTORY, Permission.CREATE_SHIPMENT,
        Permission.RISKS)]
    [SwaggerOperation(Summary = "Consulta un producto.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(ProductResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id, CancellationToken ct)
    {
        var result = await queries.HandleAsync(new GetProductByIdQuery(id), ct);
        return result.Fold<IActionResult>(product => Ok(ProductResourceAssembler.ToResource(product)),
            InventoryErrorAssembler.ToActionResult);
    }

    [HttpPost("{id:int}/lots")]
    [RequirePermission(Permission.WRITE_INVENTORY)]
    [SwaggerOperation(Summary = "Crea un lote del producto.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(LotResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateLot(int id, [FromBody] CreateLotResource resource, CancellationToken ct)
    {
        var result = await commands.HandleAsync(LotResourceAssembler.ToCommand(id, resource), ct);
        return result.Fold<IActionResult>(lot => CreatedAtAction(nameof(LotsController.GetLot), "Lots",
            new { id = lot.Id }, LotResourceAssembler.ToResource(lot)), InventoryErrorAssembler.ToActionResult);
    }

    [HttpGet("{id:int}/lots")]
    [RequirePermission(Permission.READ_INVENTORY, Permission.WRITE_INVENTORY, Permission.CREATE_SHIPMENT)]
    [SwaggerOperation(Summary = "Lista los lotes del producto.", Tags = new[] { "Inventory" })]
    [ProducesResponseType(typeof(LotResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductLots(int id, CancellationToken ct)
    {
        var result = await queries.HandleAsync(new GetProductLotsQuery(id), ct);
        return result.Fold<IActionResult>(lots => Ok(lots.Select(LotResourceAssembler.ToResource)),
            InventoryErrorAssembler.ToActionResult);
    }
}
