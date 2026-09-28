-- database.sql
-- SafeVault schema. Values are never concatenated into SQL statements against
-- this table; all application code accesses it through parameterized queries
-- (see Data/UserRepository.cs).
--
-- PasswordHash stores a bcrypt hash (see Auth/PasswordHasher.cs) - the
-- plaintext password is never written to the database. Role drives
-- role-based authorization (see Auth/AuthorizationService.cs).

CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'User'
);
