namespace SafeFlow.API.Iam.Domain.Model.Aggregates;

public sealed class RevokedToken
{
    public int Id { get; private set; }
    public string Jti { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }

    private RevokedToken() { }

    public static RevokedToken Revoke(string jti, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(jti) || jti.Length > 64) throw new ArgumentException("La sesión no es válida.");
        return new RevokedToken { Jti = jti, ExpiresAt = expiresAt.ToUniversalTime() };
    }
}
