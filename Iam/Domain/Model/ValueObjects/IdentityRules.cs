using System.Text.RegularExpressions;
using SafeFlow.API.Shared.Domain.Model.Security;

namespace SafeFlow.API.Iam.Domain.Model.ValueObjects;

public static class IdentityRules
{
    public static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public static bool ValidEmail(string email) => email.Length <= 320
        && Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.CultureInvariant);

    public static bool ValidPassword(string? password) => password is { Length: >= 8 }
        && password.Any(char.IsLetter) && password.Any(char.IsDigit)
        && password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

    public static IReadOnlySet<Permission> Defaults(Role role) => role switch
    {
        Role.OPERADOR => new HashSet<Permission>
        {
            Permission.WRITE_INVENTORY, Permission.READ_INVENTORY, Permission.READ_SHIPMENTS,
            Permission.CREATE_SHIPMENT, Permission.DELIVER
        },
        Role.SUPERVISOR => new HashSet<Permission>
        {
            Permission.SUMMARY, Permission.RISKS, Permission.ALERTS, Permission.RESOLVE
        },
        _ => new HashSet<Permission>()
    };
}
