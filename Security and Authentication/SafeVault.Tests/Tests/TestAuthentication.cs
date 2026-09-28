using SafeVault.Auth;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthentication
{
    private const string ValidPassword = "ValidPass123";

    // --- Password hashing --------------------------------------------------

    [Test]
    public void HashPassword_NeverStoresOrReturnsPlaintext()
    {
        var hash = PasswordHasher.Hash(ValidPassword);

        Assert.That(hash, Does.Not.Contain(ValidPassword));
    }

    [Test]
    public void HashPassword_SamePasswordTwice_ProducesDifferentHashes()
    {
        // bcrypt generates a fresh random salt per call, so two users with
        // the same password never end up with identical stored hashes.
        var hash1 = PasswordHasher.Hash(ValidPassword);
        var hash2 = PasswordHasher.Hash(ValidPassword);

        Assert.That(hash1, Is.Not.EqualTo(hash2));
        Assert.That(PasswordHasher.Verify(ValidPassword, hash1), Is.True);
        Assert.That(PasswordHasher.Verify(ValidPassword, hash2), Is.True);
    }

    [Test]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = PasswordHasher.Hash(ValidPassword);

        Assert.That(PasswordHasher.Verify("WrongPass456", hash), Is.False);
    }

    [Test]
    public void VerifyPassword_MalformedHash_ReturnsFalseInsteadOfThrowing()
    {
        Assert.That(PasswordHasher.Verify(ValidPassword, "not-a-real-bcrypt-hash"), Is.False);
    }

    // --- Login / authentication --------------------------------------------

    [Test]
    public void Login_ValidCredentials_Succeeds()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);
        var auth = new AuthenticationService(repository);

        var result = auth.Login("jsmith", ValidPassword);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.User, Is.Not.Null);
        Assert.That(result.User!.Username, Is.EqualTo("jsmith"));
    }

    [Test]
    public void Login_WrongPassword_Fails()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);
        var auth = new AuthenticationService(repository);

        var result = auth.Login("jsmith", "TotallyWrongPass1");

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.User, Is.Null);
        Assert.That(result.Error, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void Login_UnknownUsername_Fails()
    {
        using var repository = TestDatabase.CreateInMemoryRepository();
        var auth = new AuthenticationService(repository);

        var result = auth.Login("nobody", ValidPassword);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.User, Is.Null);
    }

    [Test]
    public void Login_UnknownUsernameAndWrongPassword_ReturnTheSameErrorMessage()
    {
        // The error message must not let a caller distinguish "this username
        // doesn't exist" from "this password is wrong" - otherwise the login
        // endpoint can be used to enumerate valid usernames.
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);
        var auth = new AuthenticationService(repository);

        var unknownUserResult = auth.Login("nobody", ValidPassword);
        var wrongPasswordResult = auth.Login("jsmith", "TotallyWrongPass1");

        Assert.That(unknownUserResult.Error, Is.EqualTo(wrongPasswordResult.Error));
    }

    [TestCase("", "somePassword1")]
    [TestCase("jsmith", "")]
    [TestCase(null, "somePassword1")]
    [TestCase("jsmith", null)]
    public void Login_BlankCredentials_Fails(string? username, string? password)
    {
        using var repository = TestDatabase.CreateInMemoryRepository();
        var auth = new AuthenticationService(repository);

        var result = auth.Login(username, password);

        Assert.That(result.IsSuccess, Is.False);
    }

    [TestCase("admin'; DROP TABLE Users; --")]
    [TestCase("' OR '1'='1")]
    public void Login_SqlInjectionInUsername_FailsSafely(string maliciousUsername)
    {
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("jsmith", "jsmith@example.com", ValidPassword);
        var auth = new AuthenticationService(repository);

        var result = auth.Login(maliciousUsername, ValidPassword);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(repository.CountUsers(), Is.EqualTo(1));
    }
}
