using SafeFlow.API.Iam.Application.Errors;
using SafeFlow.API.Iam.Application.Outbound;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Application.Patterns;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Iam.Application.Internal.CommandServices;

public sealed class IdentityCommandService(
    IUserRepository users, IRevokedTokenRepository revokedTokens, IHashingService hashing,
    ITokenService tokens, IRolePermissionQueryService permissions, IUnitOfWork unitOfWork)
    : IIdentityCommandService
{
    public async Task<Result<UserView, IamError>> Handle(SignUpCommand command, CancellationToken ct = default)
    {
        var email = IdentityRules.NormalizeEmail(command.Email);
        if (!IdentityRules.ValidEmail(email)) return new Result<UserView, IamError>.Failure(IamError.InvalidEmail);
        if (!IdentityRules.ValidPassword(command.Password)) return new Result<UserView, IamError>.Failure(IamError.InvalidPassword);
        if (!command.Terms) return new Result<UserView, IamError>.Failure(IamError.TermsRequired);
        if (await users.FindByEmailAsync(email, ct) is not null)
            return new Result<UserView, IamError>.Failure(IamError.DuplicateEmail);

        var user = User.Register(command, hashing.HashPassword(command.Password), DateTimeOffset.UtcNow);
        await users.AddAsync(user, ct);
        var granted = await permissions.EffectivePermissionsAsync(user.Role, ct);
        await unitOfWork.CompleteAsync(ct);
        return new Result<UserView, IamError>.Success(new UserView(user.Id, user.Email, user.Role, granted));
    }

    public async Task<Result<SessionView, IamError>> Handle(SignInCommand command, CancellationToken ct = default)
    {
        var user = await users.FindByEmailAsync(IdentityRules.NormalizeEmail(command.Email), ct);
        if (user is null) return new Result<SessionView, IamError>.Failure(IamError.InvalidCredentials);
        var now = DateTimeOffset.UtcNow;
        if (user.IsBlocked(now)) return new Result<SessionView, IamError>.Failure(IamError.Blocked);

        if (!hashing.VerifyPassword(command.Password ?? string.Empty, user.PasswordHash))
        {
            user.RecordFailedSignIn(now);
            // Commit the failed attempt before returning the denial (including the fifth attempt).
            await unitOfWork.CompleteAsync(ct);
            return new Result<SessionView, IamError>.Failure(user.IsBlocked(now)
                ? IamError.NewlyBlocked : IamError.InvalidCredentials);
        }

        user.RecordSuccessfulSignIn(now);
        var granted = await permissions.EffectivePermissionsAsync(user.Role, ct);
        var token = tokens.Generate(user);
        await unitOfWork.CompleteAsync(ct);
        return new Result<SessionView, IamError>.Success(new SessionView(
            new UserView(user.Id, user.Email, user.Role, granted), token.Token, token.ExpiresAt));
    }

    public async Task<Result<bool, IamError>> Handle(SignOutCommand command, CancellationToken ct = default)
    {
        if (!await revokedTokens.ContainsAsync(command.Jti, ct))
            await revokedTokens.AddAsync(RevokedToken.Revoke(command.Jti, command.ExpiresAt), ct);
        await unitOfWork.CompleteAsync(ct);
        return new Result<bool, IamError>.Success(true);
    }

    public async Task<Result<bool, IamError>> Handle(RequestRecoveryCommand command, CancellationToken ct = default)
    {
        var email = IdentityRules.NormalizeEmail(command.Email);
        if (!IdentityRules.ValidEmail(email)) return new Result<bool, IamError>.Failure(IamError.InvalidEmail);
        var user = await users.FindByEmailAsync(email, ct);
        user?.RequestRecovery(DateTimeOffset.UtcNow);
        await unitOfWork.CompleteAsync(ct);
        // Same response for unknown accounts; no password change or session is issued.
        return new Result<bool, IamError>.Success(true);
    }
}
