using System.Threading;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task<User?> UpdateAsync(User user, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}