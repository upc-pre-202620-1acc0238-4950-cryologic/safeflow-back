namespace SafeFlow.API.Iam.Domain.Model.Commands;

public sealed record SignOutCommand(string Jti, DateTimeOffset ExpiresAt);
