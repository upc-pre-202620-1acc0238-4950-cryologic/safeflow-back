namespace SafeFlow.API.Shared.Interfaces.REST.Resources;

/// <summary>Uniform error body: stable machine code plus a Spanish user-facing message.</summary>
public sealed record ErrorResource(string Error, string Message);
