using SafeVault.Data;

namespace SafeVault.Auth;

/// <summary>
/// Outcome of a login attempt. Deliberately carries a single generic error
/// message for every failure mode (unknown user, wrong password, blank
/// input) - see <see cref="AuthenticationService.Login"/> for why.
/// </summary>
public sealed class AuthenticationResult
{
    public bool IsSuccess { get; }
    public User? User { get; }
    public string? Error { get; }

    private AuthenticationResult(bool isSuccess, User? user, string? error)
    {
        IsSuccess = isSuccess;
        User = user;
        Error = error;
    }

    public static AuthenticationResult Succeeded(User user) => new(true, user, null);

    public static AuthenticationResult Failed(string error) => new(false, null, error);
}
