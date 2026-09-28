namespace SafeVault.Auth;

/// <summary>
/// Wraps bcrypt so passwords are never stored or compared in plaintext.
/// bcrypt automatically generates and embeds a random salt per password
/// (so two users with the same password get different hashes) and its work
/// factor makes brute-forcing deliberately slow.
/// </summary>
public static class PasswordHasher
{
    // Higher = slower to compute = harder to brute-force. 12 is a common,
    // reasonable default for interactive login as of 2026 hardware.
    private const int WorkFactor = 12;

    public static string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    /// <summary>
    /// Compares a plaintext password against a stored hash using bcrypt's
    /// constant-time comparison, so response time doesn't leak how much of
    /// the password matched.
    /// </summary>
    public static bool Verify(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Malformed/foreign hash in storage - treat as "does not match"
            // rather than letting the exception propagate.
            return false;
        }
    }
}
