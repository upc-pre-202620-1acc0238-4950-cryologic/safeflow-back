using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Queries;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Iam.Application.Internal.QueryServices;

public sealed class IdentityQueryService(IUserRepository users, IRolePermissionQueryService permissions)
    : IIdentityQueryService
{
    public async Task<Result<UserView, IamError>> Handle(GetCurrentUserQuery query, CancellationToken ct = default)
    {
        var user = await users.FindByIdAsync(query.UserId, ct);
        if (user is null) return new Result<UserView, IamError>.Failure(IamError.UserNotFound);
        var granted = await permissions.EffectivePermissionsAsync(user.Role, ct);
        return new Result<UserView, IamError>.Success(new UserView(user.Id, user.Email, user.Role, granted));
    }
}
