namespace SafeFlow.API.Iam.Domain.Model.Commands;

public sealed record SignUpCommand(string Email, string Password, bool Terms);
