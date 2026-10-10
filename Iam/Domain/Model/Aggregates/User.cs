using SafeFlow.API.Iam.Domain.Model.Commands;
using SafeFlow.API.Iam.Domain.Model.ValueObjects;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Domain.Model.Aggregates;

public sealed class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Role Role { get; private set; }
    public int Failures { get; private set; }
    public DateTimeOffset? BlockedUntil { get; private set; }
    public DateTimeOffset? VerificationRequestedAt { get; private set; }
    public DateTimeOffset? RecoveryRequestedAt { get; private set; }

    private User() { }

    public static User Register(SignUpCommand command, string passwordHash, DateTimeOffset requestedAt)
    {
        if (!IdentityRules.ValidPassword(command.Password))
            throw new ArgumentException("La contraseña debe tener 8 caracteres como mínimo, una letra, un número y un símbolo.");
        if (!command.Terms) throw new ArgumentException("Acepta los términos para crear tu cuenta.");
        return Create(command.Email, passwordHash, Role.OPERADOR, requestedAt);
    }

    public static User Seed(string email, string passwordHash, Role role) => Create(email, passwordHash, role, null);

    private static User Create(string email, string passwordHash, Role role, DateTimeOffset? requestedAt)
    {
        email = IdentityRules.NormalizeEmail(email);
        if (!IdentityRules.ValidEmail(email)) throw new ArgumentException("Ingresa un correo electrónico válido.");
        if (string.IsNullOrWhiteSpace(passwordHash) || passwordHash.Length > 100)
            throw new ArgumentException("La contraseña es obligatoria.");
        if (!Enum.IsDefined(role)) throw new ArgumentException("El rol no es válido.");
        return new User
        {
            Email = email, PasswordHash = passwordHash, Role = role,
            VerificationRequestedAt = requestedAt?.ToUniversalTime()
        };
    }

    public bool IsBlocked(DateTimeOffset now) => BlockedUntil > now;

    public void RecordFailedSignIn(DateTimeOffset now)
    {
        if (IsBlocked(now)) throw new InvalidOperationException("El acceso está bloqueado temporalmente.");
        Failures = BlockedUntil.HasValue ? 1 : Failures + 1;
        BlockedUntil = Failures >= 5 ? now.ToUniversalTime().AddMinutes(5) : null;
    }

    public void RecordSuccessfulSignIn(DateTimeOffset now)
    {
        if (IsBlocked(now)) throw new InvalidOperationException("El acceso está bloqueado temporalmente.");
        Failures = 0;
        BlockedUntil = null;
    }

    public void RequestRecovery(DateTimeOffset now) => RecoveryRequestedAt = now.ToUniversalTime();
}
