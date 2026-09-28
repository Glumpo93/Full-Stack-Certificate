namespace SafeVault.Auth;

/// <summary>
/// Minimum strength requirements for a plaintext password before it is
/// hashed and stored. This is unrelated to InputValidator's rules: a
/// password is never concatenated into SQL or rendered as HTML, so it does
/// not need to avoid special characters - if anything, it should encourage
/// them.
/// </summary>
public static class PasswordPolicy
{
    public const int MinLength = 8;

    public static bool IsValid(string? password, out string reason)
    {
        if (string.IsNullOrEmpty(password))
        {
            reason = "Password is required.";
            return false;
        }

        if (password.Length < MinLength)
        {
            reason = $"Password must be at least {MinLength} characters long.";
            return false;
        }

        if (!password.Any(char.IsUpper))
        {
            reason = "Password must contain at least one uppercase letter.";
            return false;
        }

        if (!password.Any(char.IsLower))
        {
            reason = "Password must contain at least one lowercase letter.";
            return false;
        }

        if (!password.Any(char.IsDigit))
        {
            reason = "Password must contain at least one digit.";
            return false;
        }

        reason = string.Empty;
        return true;
    }
}
