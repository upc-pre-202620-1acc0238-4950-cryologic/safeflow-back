namespace SafeFlow.API.Shared.Infrastructure.Interfaces.ASP.Security;

/// <summary>Skips token validation in the IAM request authorization middleware.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowAnonymousAttribute : Attribute;
