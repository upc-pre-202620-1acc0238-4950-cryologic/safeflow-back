using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SafeFlow.API.Iam.Application.Outbound;
using SafeFlow.API.Iam.Domain.Model.Aggregates;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Infrastructure.Tokens.Jwt.Services;

public sealed class TokenService(IOptions<TokenSettings> settings) : ITokenService
{
    private readonly SymmetricSecurityKey _key = new(Encoding.UTF8.GetBytes(settings.Value.Secret));

    public SessionToken Generate(User user)
    {
        var now = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var expiresAt = now.AddMinutes(30);
        var jwt = new JwtSecurityToken(
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            ],
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));
        return new SessionToken(new JwtSecurityTokenHandler().WriteToken(jwt), expiresAt);
    }

    public ValidatedSession? Validate(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                RequireSignedTokens = true,
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var securityToken);

            var sid = SingleClaim(principal, JwtRegisteredClaimNames.Sid);
            var email = SingleClaim(principal, JwtRegisteredClaimNames.Email);
            var roleName = SingleClaim(principal, "role");
            var jti = SingleClaim(principal, JwtRegisteredClaimNames.Jti);
            if (securityToken is not JwtSecurityToken jwt
                || !int.TryParse(sid, NumberStyles.None, CultureInfo.InvariantCulture, out var userId) || userId <= 0
                || !IdentityRules.ValidEmail(email ?? string.Empty)
                || !Enum.TryParse<Role>(roleName, out var role) || !Enum.IsDefined(role) || role.ToString() != roleName
                || jti is not { Length: > 0 and <= 64 }) return null;

            return new ValidatedSession(userId, email!, role, jti,
                new DateTimeOffset(DateTime.SpecifyKind(jwt.ValidTo, DateTimeKind.Utc)));
        }
        catch (SecurityTokenException) { return null; }
        catch (ArgumentException) { return null; }
    }

    private static string? SingleClaim(ClaimsPrincipal principal, string type)
    {
        var claims = principal.FindAll(type).ToArray();
        return claims.Length == 1 ? claims[0].Value : null;
    }
}
