using Microsoft.Data.Sqlite;
using SafeVault.Data;

namespace SafeVault.Tests;

/// <summary>
/// A UserRepository backed by a private, in-memory SQLite database for tests.
/// SQLite's ":memory:" database is normally destroyed as soon as its one
/// connection closes; using a shared-cache connection string plus an "anchor"
/// connection kept open for the lifetime of the instance lets UserRepository
/// open and close its own short-lived connections (exactly as it would
/// against a real database) while the data persists across those calls.
/// </summary>
public sealed class InMemoryUserRepository : UserRepository, IDisposable
{
    private readonly SqliteConnection _anchor;

    private InMemoryUserRepository(string connectionString) : base(connectionString)
    {
        _anchor = new SqliteConnection(connectionString);
        _anchor.Open();
    }

    public static InMemoryUserRepository Create()
    {
        var connectionString = $"Data Source=safevault-test-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        var repository = new InMemoryUserRepository(connectionString);
        repository.EnsureSchema();
        return repository;
    }

    public void Dispose() => _anchor.Dispose();
}

public static class TestDatabase
{
    public static InMemoryUserRepository CreateInMemoryRepository() => InMemoryUserRepository.Create();
}
