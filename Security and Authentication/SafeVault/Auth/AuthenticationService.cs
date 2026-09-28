using SafeVault.Data;

namespace SafeVault.Auth;

/// <summary>
/// Verifies login credentials against stored (hashed) passwords.
/// </summary>
public sealed class AuthenticationService
{
    private const string GenericFailureMessage = "Invalid username or password.";

    // A valid bcrypt hash of an unguessable password that no real user has.
    // Used to run the same expensive hash comparison for unknown usernames
    // as for known ones, so a timing side-channel can't reveal whether a
    // given username exists in the system.
    private static readonly string DummyHash =
        PasswordHasher.Hash("no-such-user-timing-safety-placeholder-8f3c1d");

    private readonly UserRepository _users;

    public AuthenticationService(UserRepository users)
    {
        _users = users;
    }

    /// <summary>
    /// Attempts to authenticate a user by username and password. Every
    /// failure path - blank input, unknown username, wrong password -
    /// returns the same generic error message, so a caller (or attacker)
    /// cannot use the response to enumerate valid usernames.
    /// </summary>
    public AuthenticationResult Login(string? username, string? password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return AuthenticationResult.Failed(GenericFailureMessage);
        }

        var user = _users.GetUserByUsername(username);
        var hashToCheck = user?.PasswordHash ?? DummyHash;
        var passwordMatches = PasswordHasher.Verify(password, hashToCheck);

        if (user is null || !passwordMatches)
        {
            return AuthenticationResult.Failed(GenericFailureMessage);
        }

        return AuthenticationResult.Succeeded(user);
    }
}
