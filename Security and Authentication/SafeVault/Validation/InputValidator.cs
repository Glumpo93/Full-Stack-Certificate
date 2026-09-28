using System.Net;
using System.Text.RegularExpressions;

namespace SafeVault.Validation;

/// <summary>
/// Validates and sanitizes user-supplied input for the SafeVault web form.
/// Validation uses allow-lists (reject anything that isn't expected) rather than
/// trying to blocklist "bad" characters, which is easy to bypass.
/// </summary>
public static class InputValidator
{
    // Letters, digits, underscore and hyphen only, 3-30 characters.
    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9_-]{3,30}$", RegexOptions.Compiled);

    // Characters that have no legitimate use in a username/email and are the
    // building blocks of SQL injection and XSS payloads.
    private static readonly Regex DangerousCharacters = new(@"[<>;'""`=()/\\]|--|/\*|\*/", RegexOptions.Compiled);

    /// <summary>
    /// True when <paramref name="username"/> is a safe, well-formed username.
    /// </summary>
    public static bool IsValidUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        return UsernamePattern.IsMatch(username) && !DangerousCharacters.IsMatch(username);
    }

    /// <summary>
    /// True when <paramref name="email"/> is a syntactically valid email address
    /// with no characters that could be used to break out of a query or a page.
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254)
        {
            return false;
        }

        if (DangerousCharacters.IsMatch(email))
        {
            return false;
        }

        try
        {
            var address = new System.Net.Mail.MailAddress(email);
            return address.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>
    /// Neutralizes an input value so it is safe to render back into HTML.
    /// This is a defense-in-depth measure for output/display; it does not
    /// replace parameterized queries for database access.
    /// </summary>
    public static string SanitizeForHtml(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return WebUtility.HtmlEncode(input.Trim());
    }
}
