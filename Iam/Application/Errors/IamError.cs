namespace SafeFlow.API.Iam.Application.Errors;

public enum IamErrorKind { InvalidInput, NotFound, Conflict, Rejected }

public sealed record IamError(string Code, string Message, IamErrorKind Kind)
{
    public static readonly IamError InvalidEmail = new("invalid_email", "Ingresa un correo electrónico válido.", IamErrorKind.InvalidInput);
    public static readonly IamError InvalidPassword = new("invalid_password", "La contraseña debe tener 8 caracteres como mínimo, una letra, un número y un símbolo.", IamErrorKind.InvalidInput);
    public static readonly IamError TermsRequired = new("terms_required", "Acepta los términos para crear tu cuenta.", IamErrorKind.InvalidInput);
    public static readonly IamError DuplicateEmail = new("email_already_registered", "El correo ya está registrado. Inicia sesión o recupera tu acceso.", IamErrorKind.Conflict);
    public static readonly IamError InvalidCredentials = new("invalid_credentials", "Correo o contraseña incorrectos.", IamErrorKind.Rejected);
    public static readonly IamError Blocked = new("access_blocked", "Acceso bloqueado temporalmente. Intenta en 5 minutos o solicita recuperar el acceso.", IamErrorKind.Rejected);
    public static readonly IamError NewlyBlocked = new("access_blocked", "Acceso bloqueado temporalmente por 5 minutos.", IamErrorKind.Rejected);
    public static readonly IamError UserNotFound = new("user_not_found", "El usuario no existe.", IamErrorKind.NotFound);
    public static readonly IamError AdminPermissions = new("admin_permissions_immutable", "La capacidad administrativa de ADMIN no se puede modificar ni delegar.", IamErrorKind.Rejected);
    public static readonly IamError InvalidPermissions = new("invalid_role_permissions", "Solo puedes asignar las operaciones propias de este rol.", IamErrorKind.Rejected);
}
