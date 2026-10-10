namespace SafeFlow.API.Iam.Interfaces.REST.Resources;

public sealed record SignUpResource(string? Email, string? Password, bool Terms);
