using SafeVault.Auth;
using SafeVault.Data;
using SafeVault.Validation;

namespace SafeVault.Features;

/// <summary>
/// Example of a protected feature/route: only users with the Admin role can
/// reach its content. Represents what a controller action or page handler
/// would do at the top of the method before doing any real work.
/// </summary>
public static class AdminDashboard
{
    /// <summary>
    /// Throws <see cref="UnauthorizedAccessException"/> for any non-admin
    /// user; otherwise returns the dashboard content (HTML-safe).
    /// </summary>
    public static string GetContent(User user)
    {
        AuthorizationService.RequireRole(user, Role.Admin);

        // This content is meant to be rendered into HTML, so any
        // user-controlled value embedded in it must be encoded here, at the
        // output boundary. InputValidator.IsValidUsername already restricts
        // what can be *written* to a safe character set, but that upstream
        // rule is a separate concern from what's safe to *render* - encoding
        // here means this stays safe even if that assumption ever changes.
        var safeUsername = InputValidator.SanitizeForHtml(user.Username);
        return $"Welcome to the SafeVault Admin Dashboard, {safeUsername}.";
    }
}
