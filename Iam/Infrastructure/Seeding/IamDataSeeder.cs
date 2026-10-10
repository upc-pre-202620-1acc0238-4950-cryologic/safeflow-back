using SafeFlow.API.Iam.Application.Outbound;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Shared.Application.Seeding;
using SafeFlow.API.Shared.Domain.Model.Security;
using SafeFlow.API.Shared.Domain.Repositories;

namespace SafeFlow.API.Iam.Infrastructure.Seeding;

public sealed class IamDataSeeder(
    IUserRepository users, IRolePermissionRepository permissions,
    IHashingService hashing, IUnitOfWork unitOfWork) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        foreach (var role in Enum.GetValues<Role>())
        {
            // Existing role users distinguish an intentionally empty granted set from first startup.
            var initialized = await users.AnyByRoleAsync(role, ct);
            var existingPermissions = await permissions.FindByRoleAsync(role, ct);
            var name = role.ToString().ToLowerInvariant();
            var email = $"{name}.test1@gmail.com";
            if (await users.FindByEmailAsync(email, ct) is null)
                await users.AddAsync(User.Seed(email, hashing.HashPassword($"@{name}test1"), role), ct);

            // Never fill holes in an existing configuration or restore a deliberately empty set.
            if (!initialized && existingPermissions.Count == 0)
                foreach (var permission in IdentityRules.Defaults(role))
                    await permissions.AddAsync(RolePermission.Grant(role, permission), ct);
        }
        await unitOfWork.CompleteAsync(ct);
    }
}
