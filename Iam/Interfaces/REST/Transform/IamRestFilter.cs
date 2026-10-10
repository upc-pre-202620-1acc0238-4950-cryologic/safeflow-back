using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Interfaces.REST.Controllers;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Iam.Interfaces.REST.Transform;

/// <summary>IAM-only translation of malformed requests and persistence conflicts.</summary>
public sealed class IamRestFilter : IActionFilter, IExceptionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        context.Result = context.Controller is RolePermissionsController
            ? IamErrorAssembler.ToActionResult(IamError.InvalidPermissions)
            : new BadRequestObjectResult(new ErrorResource("invalid_request", "Los datos de la solicitud no son válidos."));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException)
        {
            context.Result = new ConflictObjectResult(new ErrorResource("concurrent_update",
                "Los datos cambiaron durante la operación. Intenta nuevamente."));
            context.ExceptionHandled = true;
        }
        else if (context.Exception is DbUpdateException { InnerException: MySqlException { Number: 1062 } } update)
        {
            context.Result = update.Entries.Any(e => e.Entity is User)
                ? IamErrorAssembler.ToActionResult(IamError.DuplicateEmail)
                : new ConflictObjectResult(new ErrorResource("duplicate_record", "El registro ya existe."));
            context.ExceptionHandled = true;
        }
    }
}
