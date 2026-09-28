namespace SafeVault.Auth;

/// <summary>
/// Roles recognized by SafeVault's authorization checks. Kept as a closed
/// enum (rather than free-form strings) so an invalid/misspelled role can
/// never silently grant or deny access.
/// </summary>
public enum Role
{
    User,
    Admin,
}
