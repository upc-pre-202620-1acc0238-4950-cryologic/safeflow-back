using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Iam.Interfaces.REST.Resources;

namespace SafeFlow.API.Iam.Interfaces.REST.Transform;

public static class IdentityAssembler
{
    public static SignUpCommand ToCommand(SignUpResource resource) =>
        new(resource.Email ?? string.Empty, resource.Password ?? string.Empty, resource.Terms);

    public static SignInCommand ToCommand(SignInResource resource) =>
        new(resource.Email ?? string.Empty, resource.Password ?? string.Empty);

    public static RequestRecoveryCommand ToCommand(RecoveryRequestResource resource) =>
        new(resource.Email ?? string.Empty);

    public static UserResource ToResource(UserView user) =>
        new(user.Id, user.Email, user.Role, user.Permissions.OrderBy(p => p).ToArray());

    public static SessionResource ToResource(SessionView session) => new(session.User.Id, session.User.Email,
        session.User.Role, session.User.Permissions.OrderBy(p => p).ToArray(), session.Token, session.ExpiresAt);
}
