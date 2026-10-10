namespace SafeFlow.API.Shared.Domain.Model.Security;

/// <summary>Business roles. VISITANTE and USUARIO AUTENTICADO are access states, not roles.</summary>
public enum Role
{
    OPERADOR,
    SUPERVISOR,
    ADMIN
}
