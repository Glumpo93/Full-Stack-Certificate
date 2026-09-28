# SafeVault

Secure coding exercise: input validation, parameterized queries, and tests
that simulate SQL injection and XSS attacks.

## Structure

- `SafeVault/webform.html` — the user-facing form. Client-side checks are a
  UX convenience only; they are not the security boundary.
- `SafeVault/database.sql` — the `Users` table schema.
- `SafeVault/Validation/InputValidator.cs` — allow-list validation for
  usernames/emails, plus `SanitizeForHtml` for safely displaying input back
  to a page (defense-in-depth against XSS).
- `SafeVault/Data/UserRepository.cs` — all database access, exclusively
  through parameterized queries (bound `@parameters`, never string
  concatenation), which is what actually prevents SQL injection.
- `SafeVault.Tests/Tests/TestInputValidation.cs` — `TestForSQLInjection` and
  `TestForXSS` fire real attack payloads (`' OR '1'='1`, `<script>...`, etc.)
  at the validator and an in-memory database, and assert they're rejected /
  neutralized.
- `SafeVault.Tests/Tests/TestUserRepository.cs` — functional tests for the
  repository (insert/lookup, rejection of invalid input, safe handling of
  special characters like apostrophes).

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
