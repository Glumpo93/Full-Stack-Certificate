# 🧩 User Management API — Project Overview

Welcome to the User Management API! This project provides a clean, extensible foundation for building and managing user data through a fully functional set of CRUD endpoints. Whether you're prototyping or preparing for production, this scaffold is designed to help you move quickly while maintaining clarity and structure.

## 🚀 What’s Included

The project is structured using a standard ASP.NET Core Web API layout, including `Program.cs`, `Controllers`, `Models`, `DTOs`, `Services`, `Repositories`, and `Extensions`. Dependency injection is configured for `IUserRepository` and `IUserService`, and Swagger is enabled for interactive API exploration.

Models and DTOs such as `User`, `CreateUserDto`, and `UpdateUserDto` are defined with validation attributes and `required` modifiers to ensure data integrity and eliminate nullability warnings. The repository layer includes an asynchronous in-memory implementation, and the `UsersController` exposes full CRUD operations for managing users.

Nullable reference warnings have been addressed, environment-specific configuration files are included, and the solution is fully compatible with the .NET CLI.

## 🤖 How GitHub Copilot Helped

GitHub Copilot integrated in Visual Studio Code played a key role in accelerating development and debugging. During the initial setup, Copilot assisted in scaffolding the project structure and generating boilerplate code in `Program.cs`, including service registration and middleware configuration.

When it came to building the API endpoints, Copilot provided contextual suggestions for controller actions, helping implement the full CRUD flow for user management. This allowed for rapid iteration while maintaining consistency with ASP.NET Core conventions.

Later, during the debugging phase, Copilot proved invaluable in identifying and resolving issues reported by TechHive Solutions. It helped surface missing validation logic, suggested error handling patterns, and flagged performance bottlenecks. Copilot’s recommendations guided the implementation of input validation, try-catch blocks, and optimized query logic—ultimately improving the reliability and robustness of the API.

## 🧪 Debugging Workflow with Copilot

### Step 1: Review the Scenario

After deploying the initial version of the User Management API, TechHive Solutions reported several bugs:

- Users were being added without proper validation.
- Errors occurred when retrieving non-existent users.
- The API occasionally crashed due to unhandled exceptions.

Your task was to debug the API using GitHub Copilot to ensure it meets the company’s reliability standards.

### Step 2: Identify Bugs

Using Copilot, the codebase was reviewed to uncover key issues:

- Missing validation for user input fields, such as empty names or invalid email formats.
- Lack of error handling when attempting to retrieve users that don’t exist.
- Performance inefficiencies in the `GET /users` endpoint.

### Step 3: Fix Bugs with Copilot

Copilot assisted in implementing targeted fixes:

- Validation logic was added to ensure only well-formed user data is accepted.
- Try-catch blocks were introduced to gracefully handle exceptions and prevent crashes.
- Query logic was streamlined to improve performance and reduce overhead.

### Step 4: Test and Validate Fixes

The API was retested with a focus on edge cases—invalid input, missing users, and unexpected conditions. Copilot’s suggestions not only helped identify the root causes but also guided the resolution process, resulting in a more stable and production-ready API.

## 🔐 Compliance Requirements

To align with TechHive Solutions’ corporate policies, the User Management API must include middleware that:

- Logs all incoming requests and outgoing responses for auditing and traceability.
- Enforces standardized error handling across all endpoints to ensure consistent client feedback and system stability.
- Secures all API endpoints using token-based authentication to protect sensitive user data and restrict access to authorized clients.

These requirements are essential for maintaining operational transparency, regulatory compliance, and application security.

## 🛠️ Getting Started

To build and run the project locally:

1. Open PowerShell in the `src` directory.
2. Run the following commands:

   ```bash
   dotnet restore
   dotnet build
   dotnet run