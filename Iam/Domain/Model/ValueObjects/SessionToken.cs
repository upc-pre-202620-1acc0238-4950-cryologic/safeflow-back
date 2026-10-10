namespace SafeFlow.API.Iam.Domain.Model.ValueObjects;

public sealed record SessionToken(string Token, DateTimeOffset ExpiresAt);

public sealed record ValidatedSession(int UserId, string Email,
    SafeFlow.API.Shared.Domain.Model.Security.Role Role, string Jti, DateTimeOffset ExpiresAt);
