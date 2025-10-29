# 🧩 User Management API — Project Overview

Welcome to the User Management API! This project provides a clean, extensible foundation for building and managing user data through a fully functional set of CRUD endpoints. Whether you're prototyping or preparing for production, this scaffold is designed to help you move quickly while maintaining clarity and structure.

## 🚀 What’s Included

The project is structured using a standard ASP.NET Core Web API layout, including `Program.cs`, `Controllers`, `Models`, `DTOs`, `Services`, `Repositories`, and `Extensions`. Dependency injection is configured for `IUserRepository` and `IUserService`, and Swagger is enabled for interactive API exploration.

Models and DTOs such as `User`, `CreateUserDto`, and `UpdateUserDto` are defined with validation attributes and `required` modifiers to ensure data integrity and eliminate nullability warnings. The repository layer includes an asynchronous in-memory implementation, and the `UsersController` exposes full CRUD operations for managing users.

Nullable reference warnings have been addressed, environment-specific configuration files are included, and the solution is fully compatible with the .NET CLI.

## 🤖 How GitHub Copilot Helped

Throughout development, GitHub Copilot integrated in Visual Studio Code played a key role in accelerating the build process. During project setup, Copilot assisted in scaffolding the initial structure and generating boilerplate code in `Program.cs`, including service registration and middleware configuration.

As the API evolved, Copilot was instrumental in drafting the CRUD endpoints for user management. By interpreting comments and method signatures, it suggested controller actions for listing users, retrieving by ID, creating, updating, and deleting records. These suggestions provided a solid starting point, allowing for rapid iteration and refinement while maintaining consistency with ASP.NET Core conventions.

Copilot’s contextual awareness and code generation capabilities helped reduce repetitive tasks and kept development focused on business logic rather than boilerplate.

## 🛠️ Getting Started

To build and run the project locally:

1. Open PowerShell in the `src` directory.
2. Run the following commands:

   ```bash
   dotnet restore
   dotnet build
   dotnet run