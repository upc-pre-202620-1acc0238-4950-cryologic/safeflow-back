using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Interfaces.REST.Resources;
using SafeFlow.API.Iam.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Iam.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/auth")]
[ServiceFilter(typeof(IamRestFilter), Order = -3000)]
public sealed class AuthController(IIdentityCommandService commands) : ControllerBase
{
    [HttpPost("sign-up")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Crea una cuenta de usuario.", Tags = new[] { "Iam" })]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(IdentityAssembler.ToCommand(resource), ct);
        return result.Fold<IActionResult>(
            user => StatusCode(StatusCodes.Status201Created, IdentityAssembler.ToResource(user)),
            IamErrorAssembler.ToActionResult);
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Crea una sesión de acceso.", Tags = new[] { "Iam" })]
    [ProducesResponseType(typeof(SessionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(IdentityAssembler.ToCommand(resource), ct);
        return result.Fold<IActionResult>(
            session => Ok(IdentityAssembler.ToResource(session)),
            IamErrorAssembler.ToActionResult);
    }

    [HttpPost("sign-out")]
    [RequireAuthenticated]
    [SwaggerOperation(Summary = "Cierra la sesión actual.", Tags = new[] { "Iam" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignOut(CancellationToken ct)
    {
        var session = (ValidatedSession)HttpContext.Items[typeof(ValidatedSession)]!;
        var result = await commands.Handle(new SignOutCommand(session.Jti, session.ExpiresAt), ct);
        return result.Fold<IActionResult>(_ => NoContent(), IamErrorAssembler.ToActionResult);
    }
}
