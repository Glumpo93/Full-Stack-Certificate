using SafeVault.Auth;
using SafeVault.Data;
using SafeVault.Features;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthorization
{
    private static User MakeUser(Role role, string username = "testuser") => new()
    {
        UserId = 1,
        Username = username,
        Email = "testuser@example.com",
        PasswordHash = "irrelevant-for-authorization-tests",
        Role = role,
    };

    [Test]
    public void HasRole_MatchingRole_ReturnsTrue()
    {
        var admin = MakeUser(Role.Admin);

        Assert.That(AuthorizationService.HasRole(admin, Role.Admin), Is.True);
    }

    [Test]
    public void HasRole_DifferentRole_ReturnsFalse()
    {
        var user = MakeUser(Role.User);

        Assert.That(AuthorizationService.HasRole(user, Role.Admin), Is.False);
    }

    [Test]
    public void RequireRole_MatchingRole_DoesNotThrow()
    {
        var admin = MakeUser(Role.Admin);

        Assert.DoesNotThrow(() => AuthorizationService.RequireRole(admin, Role.Admin));
    }

    [Test]
    public void RequireRole_DifferentRole_ThrowsUnauthorizedAccessException()
    {
        var user = MakeUser(Role.User);

        Assert.Throws<UnauthorizedAccessException>(() => AuthorizationService.RequireRole(user, Role.Admin));
    }

    // --- Admin Dashboard: an example protected feature ----------------------

    [Test]
    public void AdminDashboard_AdminUser_ReturnsContent()
    {
        var admin = MakeUser(Role.Admin);

        var content = AdminDashboard.GetContent(admin);

        Assert.That(content, Does.Contain("Admin Dashboard"));
    }

    [Test]
    public void AdminDashboard_RegularUser_ThrowsUnauthorizedAccessException()
    {
        var user = MakeUser(Role.User);

        Assert.Throws<UnauthorizedAccessException>(() => AdminDashboard.GetContent(user));
    }

    [TestCase("<script>alert('XSS')</script>")]
    [TestCase("<img src=x onerror=alert(1)>")]
    public void AdminDashboard_UsernameContainingScriptTag_IsEncodedInOutput(string maliciousUsername)
    {
        // AdminDashboard.GetContent must stay safe on its own, independent
        // of whatever wrote this User object. Building the User directly
        // here (rather than through UserRepository.InsertUser, which would
        // reject this username outright) simulates a value that reached the
        // dashboard through some other path and proves the output encoding
        // - not just upstream input validation - is what makes this safe.
        var admin = MakeUser(Role.Admin, username: maliciousUsername);

        var content = AdminDashboard.GetContent(admin);

        Assert.That(content, Does.Not.Contain("<script"));
        Assert.That(content, Does.Not.Contain("<img"));
        Assert.That(content, Does.Not.Contain("<"));
    }

    [Test]
    public void AdminDashboard_EndToEnd_OnlyAdminLoginCanReachIt()
    {
        // Registers one admin and one regular user, logs each in through the
        // real authentication path, and confirms the dashboard only opens
        // for the admin's session.
        using var repository = TestDatabase.CreateInMemoryRepository();
        repository.InsertUser("admin", "admin@example.com", "AdminPass123", Role.Admin);
        repository.InsertUser("alice", "alice@example.com", "AlicePass123", Role.User);
        var auth = new AuthenticationService(repository);

        var adminLogin = auth.Login("admin", "AdminPass123");
        var aliceLogin = auth.Login("alice", "AlicePass123");

        Assert.That(adminLogin.IsSuccess, Is.True);
        Assert.That(aliceLogin.IsSuccess, Is.True);

        Assert.DoesNotThrow(() => AdminDashboard.GetContent(adminLogin.User!));
        Assert.Throws<UnauthorizedAccessException>(() => AdminDashboard.GetContent(aliceLogin.User!));
    }
}
