using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Domain.Model.Aggregates;

public sealed class RolePermission
{
    public int Id { get; private set; }
    public Role Role { get; private set; }
    public Permission Permission { get; private set; }

    private RolePermission() { }

    public static RolePermission Grant(Role role, Permission permission)
    {
        if (role == Role.ADMIN)
            throw new ArgumentException("La capacidad administrativa de ADMIN no se puede modificar ni delegar.");
        if (!IdentityRules.Defaults(role).Contains(permission))
            throw new ArgumentException("Solo puedes asignar las operaciones propias de este rol.");
        return new RolePermission { Role = role, Permission = permission };
    }
}
