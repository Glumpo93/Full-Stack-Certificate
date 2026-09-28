using SafeVault.Data;

namespace SafeVault.Tests;

[TestFixture]
public class TestUserRepository
{
    private const string ValidPassword = "ValidPass123";

    [Test]
    public void InsertUser_ThenGetByUsername_ReturnsMatchingUser()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);
        var user = repository.GetUserByUsername("jsmith");

        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Username, Is.EqualTo("jsmith"));
        Assert.That(user.Email, Is.EqualTo("jsmith@example.com"));
    }

    [Test]
    public void InsertUser_NeverStoresPlaintextPassword()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);
        var user = repository.GetUserByUsername("jsmith");

        Assert.That(user!.PasswordHash, Does.Not.Contain(ValidPassword));
        Assert.That(user.PasswordHash, Does.StartWith("$2")); // bcrypt hash prefix
    }

    [Test]
    public void GetUserByUsername_UnknownUser_ReturnsNull()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        Assert.That(repository.GetUserByUsername("nobody"), Is.Null);
    }

    [Test]
    public void InsertUser_RejectsInvalidUsername_BeforeTouchingDatabase()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        Assert.Throws<ArgumentException>(() => repository.InsertUser("a", "valid@example.com", ValidPassword));
        Assert.That(repository.CountUsers(), Is.EqualTo(0));
    }

    [Test]
    public void InsertUser_RejectsInvalidEmail_BeforeTouchingDatabase()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        Assert.Throws<ArgumentException>(() => repository.InsertUser("validUser", "not-an-email", ValidPassword));
        Assert.That(repository.CountUsers(), Is.EqualTo(0));
    }

    [TestCase("short1A")] // too short
    [TestCase("alllowercase1")] // no uppercase
    [TestCase("ALLUPPERCASE1")] // no lowercase
    [TestCase("NoDigitsHere")] // no digit
    public void InsertUser_RejectsWeakPassword_BeforeTouchingDatabase(string weakPassword)
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        Assert.Throws<ArgumentException>(() => repository.InsertUser("validUser", "valid@example.com", weakPassword));
        Assert.That(repository.CountUsers(), Is.EqualTo(0));
    }

    [Test]
    public void GetUserByUsername_WithApostropheInSearchTerm_IsTreatedAsLiteralText()
    {
        // A username containing an apostrophe would break a naive, concatenated
        // SQL string. Because the query is parameterized, it is just a search
        // for that literal value and returns no match instead of throwing or
        // executing unintended SQL.
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);

        Assert.DoesNotThrow(() => repository.GetUserByUsername("o'brien"));
        Assert.That(repository.GetUserByUsername("o'brien"), Is.Null);
    }
}
