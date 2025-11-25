namespace SGE.Application.DTOs.Users;

public class AuthResponseDto
{
    /// <summary>
    ///     Represents the access token issued for authentication purposes.
    ///     This token is used to grant the user access to protectedresources or endpoints.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    ///     Represents the refresh token issued for maintaining anauthenticated session.
    ///     This token is used to obtain a new access token after the currentaccess token expires.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates the date and time at which the current token willexpire.
    ///     This property helps determine the validity period of the tokenand when it should be refreshed.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    ///     Represents the user associated with the authentication response.
    ///     Contains details about the user's identity, roles, and personalinformation.
    /// </summary>
    public UserDto? User { get; set; }
}