using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Inventory.Interfaces.REST.Controllers;

/// <summary>Runs before ApiController's validation filter to keep malformed requests in the error contract.</summary>
public sealed class InventoryRequestValidationFilter : IActionFilter, IOrderedFilter
{
    public int Order => -3000;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        var income = context.ActionDescriptor.RouteValues.TryGetValue("action", out var action)
            && action == "RecordIncome";
        context.Result = new BadRequestObjectResult(new ErrorResource(
            income ? "invalid_income" : "invalid_request",
            income ? "La cantidad debe ser un entero mayor que cero."
                : "Los datos de la solicitud no son válidos. Completa los campos con el formato esperado."));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
