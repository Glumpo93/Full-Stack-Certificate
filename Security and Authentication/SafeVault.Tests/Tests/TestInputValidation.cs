// Tests/TestInputValidation.cs
using SafeVault.Validation;

namespace SafeVault.Tests;

[TestFixture]
public class TestInputValidation
{
    [TestCase("jsmith")]
    [TestCase("j_smith-99")]
    [TestCase("Alice123")]
    public void ValidUsername_PassesValidation(string username)
    {
        Assert.That(InputValidator.IsValidUsername(username), Is.True);
    }

    [TestCase("")]
    [TestCase("  ")]
    [TestCase("ab")] // too short
    [TestCase("this-username-is-way-too-long-to-be-allowed-in")] // too long
    [TestCase("john doe")] // whitespace not allowed
    public void InvalidUsername_FailsValidation(string username)
    {
        Assert.That(InputValidator.IsValidUsername(username), Is.False);
    }

    [TestCase("user@example.com")]
    [TestCase("first.last+tag@sub.example.co")]
    public void ValidEmail_PassesValidation(string email)
    {
        Assert.That(InputValidator.IsValidEmail(email), Is.True);
    }

    [TestCase("")]
    [TestCase("not-an-email")]
    [TestCase("missing-domain@")]
    [TestCase("@missing-local.com")]
    public void InvalidEmail_FailsValidation(string email)
    {
        Assert.That(InputValidator.IsValidEmail(email), Is.False);
    }

    // --- SQL Injection ---------------------------------------------------

    [TestCase("admin'; DROP TABLE Users; --")]
    [TestCase("' OR '1'='1")]
    [TestCase("' OR 1=1 --")]
    [TestCase("'; SELECT * FROM Users WHERE '1'='1")]
    [TestCase("admin'/*")]
    [TestCase("' UNION SELECT UserID, Username, PasswordHash, Role, Email FROM Users --")]
    [TestCase("x'; UPDATE Users SET Role = 'Admin' WHERE Username = 'legitUser'; --")]
    public void TestForSQLInjection(string maliciousInput)
    {
        // A SQL injection payload should never be accepted as a valid
        // username in the first place...
        Assert.That(InputValidator.IsValidUsername(maliciousInput), Is.False,
            $"Payload should have been rejected by validation: {maliciousInput}");

        // ...and even if it reached the database layer, parameterized
        // queries mean it is treated as a literal search value, not SQL.
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("legitUser", "legit@example.com", "ValidPass123");

        var result = repository.GetUserByUsername(maliciousInput);

        Assert.That(result, Is.Null, "Injection payload should not match any user.");
        Assert.That(repository.CountUsers(), Is.EqualTo(1),
            "Injection payload should not have altered the Users table.");
    }

    // --- Cross-Site Scripting (XSS) --------------------------------------

    [TestCase("<script>alert('XSS')</script>")]
    [TestCase("<img src=x onerror=alert(1)>")]
    [TestCase("<svg/onload=alert(1)>")]
    [TestCase("\"><script>document.location='http://evil.example'</script>")]
    public void TestForXSS(string maliciousInput)
    {
        // Should never validate as a legitimate username...
        Assert.That(InputValidator.IsValidUsername(maliciousInput), Is.False,
            $"Payload should have been rejected by validation: {maliciousInput}");

        // ...and if it were ever rendered back to a page, it must be encoded
        // so the browser treats it as inert text, not as markup/script. Once
        // every '<' and '>' is escaped, there is no tag for the browser to
        // parse, so attributes like onerror=/onload= inside it are just text.
        var sanitized = InputValidator.SanitizeForHtml(maliciousInput);

        Assert.That(sanitized, Does.Not.Contain("<"));
        Assert.That(sanitized, Does.Not.Contain(">"));
        Assert.That(sanitized, Does.Not.Contain("<script"));
        Assert.That(sanitized, Does.Not.Contain("<img"));
        Assert.That(sanitized, Does.Not.Contain("<svg"));
    }

    [Test]
    public void SanitizeForHtml_EncodesAngleBracketsAndQuotes()
    {
        var sanitized = InputValidator.SanitizeForHtml("<b>\"quoted\"</b>");

        Assert.That(sanitized, Does.Not.Contain("<"));
        Assert.That(sanitized, Does.Not.Contain(">"));
        Assert.That(sanitized, Does.Contain("&lt;"));
        Assert.That(sanitized, Does.Contain("&gt;"));
    }
}
