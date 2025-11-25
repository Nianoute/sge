namespace SGE.Core.Exceptions;

public class UserAlreadyExistsException : SgeException
{
    public UserAlreadyExistsException(string identifier, string type =
        "email")
        : base($"Un utilisateur avec ce {type} '{identifier}' existe déjà.", "USER_ALREADY_EXISTS", 409)
    {
    }
}
public class UserRegistrationException : SgeException
{
    public UserRegistrationException(IEnumerable<string> errors)
        : base($"Erreur lors de la création de l'utilisateur: {string.Join("; ", errors)}", "USER_REGISTRATION_FAILED", 400)
    {
    }
}
public class InvalidCredentialsException : SgeException
{
    public InvalidCredentialsException()
        : base("Email ou mot de passe incorrect.",
            "INVALID_CREDENTIALS", 401)
    {
    }
}
public class TokenExpiredException : SgeException
{
    public TokenExpiredException()
        : base("Le token a expiré.", "TOKEN_EXPIRED", 401)
    {
    }
}
public class InvalidRefreshTokenException : SgeException
{
    public InvalidRefreshTokenException()
        : base("Token de rafraîchissement invalide.",
            "INVALID_REFRESH_TOKEN", 401)
    {
    }
}
public class UserNotActiveException : SgeException
{
    public UserNotActiveException()
        : base("Compte utilisateur désactivé.", "USER_NOT_ACTIVE", 403)
    {
    }
}