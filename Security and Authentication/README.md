# SafeVault

Secure coding exercise: input validation, parameterized queries, password
hashing, role-based authorization, and tests that simulate SQL injection,
XSS, invalid logins, and unauthorized access.

## Structure

- `SafeVault/webform.html` — the registration form. Client-side checks are a
  UX convenience only; they are not the security boundary.
- `SafeVault/loginform.html` — the login form (username + password).
- `SafeVault/database.sql` — the `Users` table schema (includes `PasswordHash`
  and `Role`).
- `SafeVault/Validation/InputValidator.cs` — allow-list validation for
  usernames/emails, plus `SanitizeForHtml` for safely displaying input back
  to a page (defense-in-depth against XSS).
- `SafeVault/Data/UserRepository.cs` — all database access, exclusively
  through parameterized queries (bound `@parameters`, never string
  concatenation), which is what actually prevents SQL injection.
- `SafeVault/Auth/PasswordPolicy.cs` — minimum password strength rules.
- `SafeVault/Auth/PasswordHasher.cs` — bcrypt hashing/verification; plaintext
  passwords are never stored or compared directly.
- `SafeVault/Auth/AuthenticationService.cs` — `Login(username, password)`.
  Every failure path (unknown user, wrong password, blank input) returns the
  same generic error and the same hashing cost, so a login attempt can't be
  used to enumerate valid usernames via message content or response timing.
- `SafeVault/Auth/AuthorizationService.cs` — role-based access control:
  `RequireRole` throws `UnauthorizedAccessException` when a user lacks the
  needed role.
- `SafeVault/Features/AdminDashboard.cs` — example protected feature, gated
  to the `Admin` role via `AuthorizationService`.
- `SafeVault.Tests/Tests/TestInputValidation.cs` — `TestForSQLInjection` and
  `TestForXSS` fire real attack payloads (`' OR '1'='1`, `<script>...`, etc.)
  at the validator and an in-memory database, and assert they're rejected /
  neutralized.
- `SafeVault.Tests/Tests/TestUserRepository.cs` — functional tests for the
  repository (insert/lookup, rejection of invalid input/weak passwords, safe
  handling of special characters like apostrophes, passwords never stored in
  plaintext).
- `SafeVault.Tests/Tests/TestAuthentication.cs` — hashing behavior, valid
  login, invalid password, unknown username, blank credentials, and SQL
  injection attempts against the login form.
- `SafeVault.Tests/Tests/TestAuthorization.cs` — role checks and an
  end-to-end test proving only an admin's session can reach the Admin
  Dashboard.

## Running the tests

```
dotnet test SafeVault.slnx
```

## Key points

- **Validation ≠ escaping ≠ parameterization.** All three are used here for
  their own purpose: validation rejects malformed input outright,
  parameterized queries are the actual defense against SQL injection, and
  HTML-encoding on output is the defense against XSS when data is later
  displayed.
- Tests use a private in-memory SQLite database, so `dotnet test` needs no
  external database server.

## Security audit (SQL injection / XSS)

Full grep of `UserRepository.cs` confirmed every query uses bound
`@parameters` with no string concatenation into `CommandText` - no SQL
injection sink found.

One gap was found and fixed: `AdminDashboard.GetContent` interpolated
`user.Username` into its "Welcome" string with no output encoding. It was
only safe by coincidence, because `InputValidator.IsValidUsername` currently
restricts stored usernames to a safe character set - a separate, upstream
concern from what's safe to render. Fixed by encoding with
`InputValidator.SanitizeForHtml` at the point of output, and covered by
`AdminDashboard_UsernameContainingScriptTag_IsEncodedInOutput`, which builds
a `User` with a `<script>`-laden username directly (bypassing
`InsertUser`'s validation) to prove the output itself is safe regardless of
how the data got there.
