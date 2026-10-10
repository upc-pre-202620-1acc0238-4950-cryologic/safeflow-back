using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Alerts.Interfaces.REST.Controllers;

/// <summary>Uses the shared error contract before ApiController's default model validation filter.</summary>
public sealed class IncidentRequestValidationFilter : IActionFilter, IOrderedFilter
{
    public int Order => -2001;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(new ErrorResource("invalid_request",
                "La solicitud contiene datos inválidos."));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
