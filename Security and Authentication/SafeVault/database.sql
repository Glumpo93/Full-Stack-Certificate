-- database.sql
-- SafeVault schema. Values are never concatenated into SQL statements against
-- this table; all application code accesses it through parameterized queries
-- (see Data/UserRepository.cs).

CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL
);
