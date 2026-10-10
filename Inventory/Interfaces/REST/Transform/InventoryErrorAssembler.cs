using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Inventory.Application.Errors;
using SafeFlow.API.Inventory.Interfaces.REST.Resources;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Inventory.Interfaces.REST.Transform;

public static class InventoryErrorAssembler
{
    public static IActionResult ToActionResult(InventoryError error)
    {
        var status = error.Code switch
        {
            "product_not_found" or "lot_not_found" => StatusCodes.Status404NotFound,
            "lot_already_exists" or "lot_concurrency_conflict" => StatusCodes.Status409Conflict,
            "invalid_condition" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status422UnprocessableEntity
        };
        var resource = new ErrorResource(error.Code, error.Message);
        object body = error.ExistingLotId is { } id
            ? new DuplicateLotErrorResource(resource.Error, resource.Message, id)
            : resource;
        return new ObjectResult(body) { StatusCode = status };
    }
}
