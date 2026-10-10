using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Interfaces.REST.Resources;

public sealed record UserResource(int Id, string Email, Role Role, IReadOnlyList<Permission> Permissions);
