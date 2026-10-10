using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Logistics.Interfaces.REST.Controllers;

// Runs before ApiController's automatic model-state response, scoped to this controller.
public sealed class ShipmentRequestValidationAttribute : ActionFilterAttribute
{
    public ShipmentRequestValidationAttribute()
    {
        Order = -2001;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(
                new ErrorResource("invalid_request", "Los datos del despacho no son válidos."));
    }
}
