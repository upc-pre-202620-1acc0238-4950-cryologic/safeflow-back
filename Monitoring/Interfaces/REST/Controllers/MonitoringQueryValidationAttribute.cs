using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Monitoring.Interfaces.REST.Controllers;

// Run before ApiController's ModelStateInvalidFilter to preserve the shared error resource.
[AttributeUsage(AttributeTargets.Class)]
public sealed class MonitoringQueryValidationAttribute : Attribute, IActionFilter, IOrderedFilter
{
    public int Order => -3000;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var invalidDate = context.ModelState.Any(entry => entry.Value?.Errors.Count > 0 &&
            (string.Equals(entry.Key, "from", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(entry.Key, "to", StringComparison.OrdinalIgnoreCase)));
        context.Result = new BadRequestObjectResult(new ErrorResource("invalid_query",
            invalidDate ? "Ingresa una fecha y hora válidas." : "Los parámetros de consulta no son válidos."));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
