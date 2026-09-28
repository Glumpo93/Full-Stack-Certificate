using SafeVault.Data;

namespace SafeVault.Auth;

/// <summary>
/// Role-based access control (RBAC) checks. Authentication (proving who you
/// are) and authorization (checking what you're allowed to do) are kept
/// deliberately separate: a caller must already hold an authenticated
/// <see cref="User"/> (e.g. from <see cref="AuthenticationService.Login"/>)
/// before any of these checks run.
/// </summary>
public static class AuthorizationService
{
    public static bool HasRole(User user, Role requiredRole) => user.Role == requiredRole;

    /// <summary>
    /// Throws <see cref="UnauthorizedAccessException"/> if <paramref name="user"/>
    /// does not have <paramref name="requiredRole"/>. Intended to guard the
    /// start of a protected route/feature/method.
    /// </summary>
    public static void RequireRole(User user, Role requiredRole)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (!HasRole(user, requiredRole))
        {
            throw new UnauthorizedAccessException(
                $"User '{user.Username}' has role '{user.Role}' but this action requires '{requiredRole}'.");
        }
    }
}
