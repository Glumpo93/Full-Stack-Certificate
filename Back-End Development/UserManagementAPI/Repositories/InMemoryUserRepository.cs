public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();
    private int _nextId = 1;

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await Task.FromResult(_users.Values);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_users.GetValueOrDefault(id));
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Id = _nextId++;
        _users[user.Id] = user;
        return await Task.FromResult(user);
    }

    public async Task<User?> UpdateAsync(User user)
    {
        if (!_users.ContainsKey(user.Id))
            return null;

        _users[user.Id] = user;
        return await Task.FromResult(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await Task.FromResult(_users.Remove(id));
    }
}