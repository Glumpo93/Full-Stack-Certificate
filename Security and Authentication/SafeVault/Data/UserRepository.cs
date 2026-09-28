using Microsoft.Data.Sqlite;
using SafeVault.Validation;

namespace SafeVault.Data;

/// <summary>
/// Data access for the Users table. Every query uses parameterized statements
/// (bound parameters), so user-supplied values are always sent to the database
/// as data and can never be interpreted as SQL, no matter what characters they
/// contain.
/// </summary>
public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates the Users table if it does not already exist. Mirrors database.sql.
    /// </summary>
    public void EnsureSchema()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username VARCHAR(100) NOT NULL,
                Email VARCHAR(100) NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Inserts a new user. Throws <see cref="ArgumentException"/> if the input
    /// does not pass validation, and uses a parameterized INSERT so the values
    /// can never alter the shape of the SQL statement.
    /// </summary>
    public int InsertUser(string username, string email)
    {
        if (!InputValidator.IsValidUsername(username))
        {
            throw new ArgumentException("Invalid username.", nameof(username));
        }

        if (!InputValidator.IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email.", nameof(email));
        }

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (Username, Email) VALUES (@username, @email);";
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@email", email);
        command.ExecuteNonQuery();

        using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";
        return Convert.ToInt32(idCommand.ExecuteScalar());
    }

    /// <summary>
    /// Looks up a user by username using a parameterized SELECT. Because the
    /// username is bound as a parameter rather than concatenated into the SQL
    /// text, classic injection payloads (e.g. "' OR '1'='1") are treated as a
    /// literal string to search for, not as SQL, so they simply match nothing.
    /// </summary>
    public User? GetUserByUsername(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT UserID, Username, Email FROM Users WHERE Username = @username;";
        command.Parameters.AddWithValue("@username", username);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new User
        {
            UserId = reader.GetInt32(0),
            Username = reader.GetString(1),
            Email = reader.GetString(2),
        };
    }

    /// <summary>
    /// Returns the number of rows currently in the Users table. Used by tests
    /// to confirm a table-dropping/altering injection payload had no effect.
    /// </summary>
    public int CountUsers()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Users;";
        return Convert.ToInt32(command.ExecuteScalar());
    }
}
