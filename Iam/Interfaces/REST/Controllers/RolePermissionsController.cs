using Microsoft.AspNetCore.Mvc;
using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Queries;
using SafeFlow.API.Iam.Interfaces.REST.Resources;
using SafeFlow.API.Iam.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;
using SafeFlow.API.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace SafeFlow.API.Iam.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/role-permissions")]
[RequireAdmin]
[ServiceFilter(typeof(IamRestFilter), Order = -3000)]
public sealed class RolePermissionsController(IRolePermissionCommandService commands, IRolePermissionQueryService queries)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lista los permisos por rol.", Tags = new[] { "Iam" })]
    [ProducesResponseType(typeof(RolePermissionsResource[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var roles = await queries.Handle(new GetRolePermissionsQuery(), ct);
        return Ok(roles.Select(RolePermissionAssembler.ToResource));
    }

    [HttpPut("{role}")]
    [SwaggerOperation(Summary = "Actualiza los permisos de un rol.", Tags = new[] { "Iam" })]
    [ProducesResponseType(typeof(RolePermissionsResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResource), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Replace(string role, [FromBody] ReplaceRolePermissionsResource resource, CancellationToken ct)
    {
        if (!Enum.TryParse<Role>(role, true, out var target) || !Enum.IsDefined(target)
            || !string.Equals(target.ToString(), role, StringComparison.OrdinalIgnoreCase))
            return IamErrorAssembler.ToActionResult(IamError.InvalidPermissions);
        if (resource.Granted is null) return IamErrorAssembler.ToActionResult(IamError.InvalidPermissions);
        var result = await commands.Handle(RolePermissionAssembler.ToCommand(target, resource), ct);
        return result.Fold<IActionResult>(permissions => Ok(RolePermissionAssembler.ToResource(permissions)), IamErrorAssembler.ToActionResult);
    }
}
