using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Interfaces.REST.Resources;
using SafeFlow.API.Iam.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Iam.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/recovery-requests")]
[ServiceFilter(typeof(IamRestFilter), Order = -3000)]
public sealed class RecoveryRequestsController(IIdentityCommandService commands) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Solicita la recuperación de acceso.", Tags = new[] { "Iam" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RequestRecovery([FromBody] RecoveryRequestResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(IdentityAssembler.ToCommand(resource), ct);
        return result.Fold<IActionResult>(_ => NoContent(), IamErrorAssembler.ToActionResult);
    }
}
