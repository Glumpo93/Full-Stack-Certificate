using SafeVault.Auth;

namespace SafeVault.Data;

public sealed class User
{
    public int UserId { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required Role Role { get; init; }
}
