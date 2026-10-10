using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Queries;
using SafeFlow.API.Iam.Interfaces.REST.Resources;
using SafeFlow.API.Iam.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Iam.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/users")]
[RequireAuthenticated]
[ServiceFilter(typeof(IamRestFilter), Order = -3000)]
public sealed class UsersController(IIdentityQueryService queries) : ControllerBase
{
    [HttpGet("me")]
    [SwaggerOperation(Summary = "Consulta el usuario de la sesión.", Tags = new[] { "Iam" })]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var result = await queries.Handle(new GetCurrentUserQuery(HttpContext.GetCurrentUser().Id), ct);
        return result.Fold<IActionResult>(user => Ok(IdentityAssembler.ToResource(user)), IamErrorAssembler.ToActionResult);
    }
}
