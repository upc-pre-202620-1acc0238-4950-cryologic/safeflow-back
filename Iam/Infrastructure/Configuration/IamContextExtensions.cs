using System.Text;
using SafeFlow.API.Iam.Application.Internal.CommandServices;
using SafeFlow.API.Iam.Application.Internal.QueryServices;
using SafeFlow.API.Iam.Application.Outbound;
using SafeFlow.API.Iam.Application.Services;
using SafeFlow.API.Iam.Domain.Repositories;
using SafeFlow.API.Iam.Infrastructure.Hashing.Services;
using SafeFlow.API.Iam.Infrastructure.Persistence.EFC.Repositories;
using SafeFlow.API.Iam.Infrastructure.Seeding;
using SafeFlow.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using SafeFlow.API.Iam.Infrastructure.Tokens.Jwt.Services;
using SafeFlow.API.Iam.Interfaces.REST.Transform;
using SafeFlow.API.Shared.Application.Seeding;

namespace SafeFlow.API.Iam.Infrastructure.Configuration;

/// <summary>Registers the IAM bounded context (identity, sessions, permissions and seed accounts).</summary>
public static class IamContextExtensions
{
    public static IServiceCollection AddIamContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TokenSettings>()
            .Bind(configuration.GetSection("TokenSettings"))
            .Validate(settings => !string.IsNullOrWhiteSpace(settings.Secret)
                && Encoding.UTF8.GetByteCount(settings.Secret) >= 32, "TokenSettings:Secret debe tener al menos 32 bytes.")
            .ValidateOnStart();
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();
        services.AddScoped<IIdentityCommandService, IdentityCommandService>();
        services.AddScoped<IIdentityQueryService, IdentityQueryService>();
        services.AddScoped<IRolePermissionCommandService, RolePermissionCommandService>();
        services.AddScoped<IRolePermissionQueryService, RolePermissionQueryService>();
        services.AddScoped<IDataSeeder, IamDataSeeder>();
        services.AddScoped<IamRestFilter>();
        return services;
    }
}
