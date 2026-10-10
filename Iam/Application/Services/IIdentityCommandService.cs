using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Shared.Application.Patterns;

namespace SafeFlow.API.Iam.Application.Services;

public interface IIdentityCommandService
{
    Task<Result<UserView, IamError>> Handle(SignUpCommand command, CancellationToken ct = default);
    Task<Result<SessionView, IamError>> Handle(SignInCommand command, CancellationToken ct = default);
    Task<Result<bool, IamError>> Handle(SignOutCommand command, CancellationToken ct = default);
    Task<Result<bool, IamError>> Handle(RequestRecoveryCommand command, CancellationToken ct = default);
}
