using SafeVault.Data;

namespace SafeVault.Tests;

[TestFixture]
public class TestUserRepository
{
    [Test]
    public void InsertUser_ThenGetByUsername_ReturnsMatchingUser()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        repository.InsertUser("jsmith", "jsmith@example.com");
        var user = repository.GetUserByUsername("jsmith");

        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Username, Is.EqualTo("jsmith"));
        Assert.That(user.Email, Is.EqualTo("jsmith@example.com"));
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

        Assert.Throws<ArgumentException>(() => repository.InsertUser("a", "valid@example.com"));
        Assert.That(repository.CountUsers(), Is.EqualTo(0));
    }

    [Test]
    public void InsertUser_RejectsInvalidEmail_BeforeTouchingDatabase()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();

        Assert.Throws<ArgumentException>(() => repository.InsertUser("validUser", "not-an-email"));
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
        repository.InsertUser("jsmith", "jsmith@example.com");

        Assert.DoesNotThrow(() => repository.GetUserByUsername("o'brien"));
        Assert.That(repository.GetUserByUsername("o'brien"), Is.Null);
    }
}
