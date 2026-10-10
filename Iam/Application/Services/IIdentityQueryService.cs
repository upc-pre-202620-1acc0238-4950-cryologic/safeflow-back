using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Domain.Model.Queries;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Iam.Application.Services;

public interface IIdentityQueryService
{
    Task<Result<UserView, IamError>> Handle(GetCurrentUserQuery query, CancellationToken ct = default);
}
