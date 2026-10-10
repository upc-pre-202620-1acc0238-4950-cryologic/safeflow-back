using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Shared.Interfaces.REST.Resources;

namespace SafeFlow.API.Iam.Interfaces.REST.Transform;

public static class IamErrorAssembler
{
    public static ObjectResult ToActionResult(IamError error) => new(new ErrorResource(error.Code, error.Message))
    {
        StatusCode = error.Kind switch
        {
            IamErrorKind.InvalidInput => StatusCodes.Status400BadRequest,
            IamErrorKind.NotFound => StatusCodes.Status404NotFound,
            IamErrorKind.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status422UnprocessableEntity
        }
    };
}
