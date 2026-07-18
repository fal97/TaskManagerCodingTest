# TaskManager Coding Test - Codex Instructions

## Project Overview
- **Project Type**: ASP.NET Core 8.0 Web API
- **Repository**: https://github.com/fal97/TaskManagerCodingTest
- **Current Branch**: Develop
- **Target Framework**: .NET 8.0 with nullable reference types and implicit usings enabled

## Tech Stack
- **Language**: C# .NET 8
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **CQRS/Mediation**: MediatR
- **Validation**: FluentValidation
- **Testing**: xUnit
- **Message Queue**: Azure Service Bus
- **Caching**: Redis

## Project Structure
```
TaskManagerCodingTest/
├── Backend/                       # Main ASP.NET Core application
│   ├── Controllers/               # API Controllers
│   ├── Program.cs                 # Application startup and configuration
│   ├── appsettings.json          # Configuration settings
│   ├── appsettings.Development.json
│   ├── Backend.http              # HTTP Client requests
│   └── Properties/               # Launch settings
```

## Architecture

### Clean Architecture Principles
Follow Clean Architecture strictly:

- **Domain Layer**: Core business logic and entities - must NOT depend on Infrastructure or API layers
- **Application Layer**: CQRS commands/queries, handlers, validators, DTOs, business rules
- **Infrastructure Layer**: EF Core implementations, external service integrations, repositories, databases
- **API Layer**: HTTP concerns only - controllers should be thin, delegating to Application layer

### Design Patterns
- **CQRS**: Use MediatR for Command/Query separation
- **Validation**: Use FluentValidation validators in Application layer
- **DTOs**: Map domain entities to DTOs for API responses
- **Repositories**: Abstract data access through repository pattern (Infrastructure layer)

## Code Standards & Conventions

### C# Style Guide
- **.NET Version**: .NET 8.0 - Use modern C# features (records, required properties, target-typed new, etc.)
- **Nullable Reference Types**: Enabled - Always mark nullable types explicitly with `?`
- **Implicit Usings**: Enabled - Leverage System namespaces without explicit using statements
- **Naming Conventions**: 
  - Classes, methods, properties: `PascalCase`
  - Private fields: `_camelCase`
  - Local variables: `camelCase`
  - Constants: `UPPER_SNAKE_CASE`
- **Async/Await**: Always use `async/await` for I/O operations
- **Cancellation Tokens**: Include `CancellationToken` parameter in all service, repository, and handler methods
- **Method Size**: Keep methods small and focused - avoid large methods
- **Dependency Injection**: Always use constructor injection, never service locator pattern
- **No Business Logic in Controllers**: Controllers should only handle HTTP concerns, delegate to Application layer
- **No DbContext in Controllers**: Access data through repositories or MediatR handlers

### Entity Framework Core Rules
- **Read-Only Queries**: Always use `AsNoTracking()` for queries that don't update data
- **Query Optimization**: Avoid N+1 queries with proper use of `Include()` and projections
- **Selective Loading**: Use `Include()` only when necessary - avoid loading unrelated data
- **DTO Projections**: Use `.Select()` projections into DTOs for read APIs instead of loading full entities
- **API Responses**: Never load full entities just to return API responses - project to DTOs at query level

## Development Workflow

### Before Making Changes
Always follow this process:
1. **Understand the Pattern**: Study existing similar implementations first
2. **Search First**: Look for similar implementations in the codebase
3. **Plan**: Briefly explain the planned change before implementing
4. **Minimal Changes**: Modify the smallest number of files necessary
5. **Test**: Run or suggest relevant tests to verify the change

### Adding New Features - MediatR/CQRS Pattern
1. **Command/Query**: Create in `Application/Features/[Domain]/Commands/` or `Queries/`
2. **Validator**: Add FluentValidation in same folder
3. **Handler**: Implement `IRequestHandler<T, R>` in handler file
4. **DTO**: Create request/response DTOs in `Application/DTOs/`
5. **Controller**: Create thin controller that uses `IMediator` to dispatch command/query
6. **Repository**: If data access needed, use repository from Infrastructure layer
7. **Tests**: Add unit tests for commands/queries and integration tests for API

### Creating Controllers
- Keep controllers thin - HTTP concerns only
- Inject `IMediator` from MediatR
- Delegate all business logic to handlers
- Use appropriate HTTP verbs: `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- Example:
  ```csharp
  [ApiController]
  [Route("api/[controller]")]
  public class TasksController(IMediator mediator) : ControllerBase
  {
      [HttpGet("{id}")]
      public async Task<IActionResult> GetTask(int id, CancellationToken ct)
          => Ok(await mediator.Send(new GetTaskQuery(id), ct));
  }
  ```

### Configuration
- Use `appsettings.json` for production configuration
- Use `appsettings.Development.json` for development overrides
- Access configuration via `IConfiguration` dependency injection in `Program.cs`

## Testing

### Unit Tests
- Test business logic in Application layer (handlers, validators, services)
- Use xUnit as testing framework
- Naming convention: `[ClassName]Tests` class, `[MethodName]_[Scenario]_[ExpectedResult]` for methods
- Mock external dependencies (repositories, external services)

### Integration Tests
- Test API endpoints and database interactions
- Test repository implementations
- Use an in-memory or test database
- Verify end-to-end behavior

### Test Organization
- Mirror the Application layer structure
- Keep tests in `Backend.Tests` project
- Use clear test names that describe the scenario and expected outcome

## Git Workflow
- **Current Branch**: Develop
- Keep commits atomic and meaningful
- Write clear commit messages describing the change
- Push changes regularly to keep work backed up

## Useful Commands

### Build
```powershell
dotnet build
```

### Run Tests
```powershell
dotnet test
```

### Code Formatting
```powershell
dotnet format
```

## Working with Codex/Copilot

### When You Ask Me To...
- **Fix code**: I'll identify root causes and provide minimal, targeted fixes
- **Add features**: I'll follow the architecture patterns and structure outlined above
- **Refactor**: I'll maintain existing patterns while improving code quality
- **Debug**: I'll search the workspace methodically before making assumptions

### Please Provide
- Context about business requirements or constraints
- Specific error messages or unexpected behaviors
- Links to external documentation if relevant
- Preferences for specific implementations or patterns

## Important Notes
- The project uses implicit global usings - no need for repetitive `using` statements
- ASP.NET Core uses top-level `Program.cs` - keep it clean and clear
- Use `Backend.http` file for quick API endpoint testing
- Ensure nullable reference types are respected throughout the codebase
