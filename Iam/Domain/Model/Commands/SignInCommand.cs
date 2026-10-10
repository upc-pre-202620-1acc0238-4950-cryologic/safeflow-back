namespace SafeFlow.API.Iam.Domain.Model.Commands;

public sealed record SignInCommand(string Email, string Password);
