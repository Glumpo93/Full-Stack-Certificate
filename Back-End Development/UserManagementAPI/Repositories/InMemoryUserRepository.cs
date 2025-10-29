using System.Collections.Concurrent;
using System.Threading;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextId = 0;

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        // Return a deterministic snapshot ordered by Id to support safe pagination
        var snapshot = _users.Values
            .OrderBy(u => u.Id)
            .Select(u => new User { Id = u.Id, Name = u.Name, Email = u.Email }) // defensive copy
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyList<User>)snapshot);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user is null ? null : new User { Id = user.Id, Name = user.Name, Email = user.Email });
    }

    public Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        // Basic uniqueness check (case-insensitive)
        if (_users.Values.Any(u => string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A user with the same email already exists.");

        user.Id = Interlocked.Increment(ref _nextId);
        // store a copy to avoid external mutation
        var stored = new User { Id = user.Id, Name = user.Name, Email = user.Email };
        _users[stored.Id] = stored;
        return Task.FromResult(stored);
    }

    public Task<User?> UpdateAsync(User user, CancellationToken ct = default)
    {
        if (!_users.ContainsKey(user.Id))
            return Task.FromResult<User?>(null);

        // Prevent duplicate email across other users
        if (_users.Values.Any(u => u.Id != user.Id && string.Equals(u.Email, user.Email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Another user with the same email already exists.");

        var stored = new User { Id = user.Id, Name = user.Name, Email = user.Email };
        _users[stored.Id] = stored;
        return Task.FromResult<User?>(stored);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(_users.TryRemove(id, out _));
    }
}