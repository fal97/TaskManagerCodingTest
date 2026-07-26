# Task Manager

Task Manager is a full-stack task management application built with ASP.NET Core and Angular. It supports authentication, task creation and editing, soft deletion, completion tracking, filtering, searching, and server-side sorting.

## Technology stack

### Backend

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server
- MediatR and CQRS
- FluentValidation
- Cookie-based authentication
- xUnit, Moq, and FluentAssertions

### Frontend

- Angular 21
- Angular Material
- Standalone components
- Reactive Forms
- Route guards and HTTP interceptors

## Repository structure

```text
TaskManagerCodingTest/
├── Backend.API/              # Controllers, middleware, authentication cookie, configuration
├── Backend.Application/      # CQRS commands, queries, handlers, validators, and DTOs
├── Backend.Domain/           # Domain entities and enums
├── Backend.Infrastructure/   # EF Core, SQL Server, authentication, migrations, and SQL script
├── Backend.Tests/            # Backend unit tests
├── Frontend/                 # Angular application
└── TaskManagerCodingTest.slnx
```

## Prerequisites

Install the following software:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) 22.19 or newer compatible with Angular 21
- SQL Server, SQL Server Express, or SQL Server LocalDB
- SQL Server Management Studio or Azure Data Studio

## Database setup

The repository includes one portable database script:

```text
Backend.Infrastructure/Scripts/DatabaseScript.sql
```

The script:

- Creates `TaskManagerDb` when it does not exist.
- Lets SQL Server select its configured data and log directories.
- Creates the EF migration history and `UserTasks` table.
- Creates the required indexes.
- Inserts sample task data only when the task table is empty.
- Contains no machine-specific `.mdf` or `.ldf` paths.

### Run the database script

1. Open SQL Server Management Studio or Azure Data Studio.
2. Connect to the SQL Server instance that will host the application database.
3. Open `Backend.Infrastructure/Scripts/DatabaseScript.sql`.
4. Execute the complete script.
5. Confirm that `TaskManagerDb` and `dbo.UserTasks` were created.

The SQL account running the script must have permission to create databases. If the database already exists, the script does not delete it.

### Configure the backend connection

The default connection in `Backend.API/appsettings.json` uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagerDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

Change `DefaultConnection` if a different SQL Server instance is used. For example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TaskManagerDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```


## Run the backend

From the repository root:

```powershell
dotnet restore
dotnet run --project Backend.API
```

The HTTPS development address is normally:

```text
https://localhost:7124
```

If the local HTTPS certificate is not trusted, run:

```powershell
dotnet dev-certs https --trust
```

## Authentication

The application uses an HTTP-only authentication cookie rather than JWT tokens.

Development credentials:

```text
Username: admin
Password: TaskManager123!
```

The password is stored in development configuration as a PBKDF2 hash and salt. Replace the development credentials before using the project outside a coding-test environment.

Authentication endpoints:

```text
POST /api/auth/login
POST /api/auth/logout
GET  /api/auth/me
```

All task endpoints require an authenticated session.

## Run the frontend

Open a second terminal:

```powershell
cd Frontend
npm install
npm start
```

Open:

```text
http://localhost:4200
```

The development API URL is configured in:

```text
Frontend/src/environments/environment.development.ts
```

If the backend address changes, update `apiBaseUrl` in that file. The frontend sends cookies with API requests, so the backend must remain configured to allow the frontend origin with credentials.

## Task API

```text
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
GET    /api/tasks/search
```

The search endpoint supports:

- Text search across title, description, and notes
- Status and priority filtering
- Due-date range filtering
- Sorting by title, status, priority, due date, created date, or modified date
- Ascending and descending order

Example:

```http
GET /api/tasks/search?searchTerm=review&priority=2&sortBy=dueDate&sortDirection=asc
```

Additional request examples are available in `Backend.API/Backend.http`.

## Email Azure Function

Task-created emails are processed by the separate `Backend.EmailFunction`
project. The API only publishes messages to the `task-created-email` Azure
Service Bus queue; the Function consumes them and sends email through SMTP.

For the API, enable publishing and provide the Service Bus connection through
user secrets or environment variables:

```powershell
dotnet user-secrets init --project Backend.API
dotnet user-secrets set "ServiceBus:Enabled" "true" --project Backend.API
dotnet user-secrets set "ServiceBus:ConnectionString" "<connection-string>" --project Backend.API
```

For local Function development, copy
`Backend.EmailFunction/local.settings.example.json` to
`Backend.EmailFunction/local.settings.json` and replace the placeholders.
`local.settings.json` is ignored by Git.

Run the Function with Azure Functions Core Tools:

```powershell
cd Backend.EmailFunction
func start
```

In Azure, add the same values from the example file as Function App application
settings. The Function uses scheduled retries with exponential delays and moves
permanently failed messages to the queue's dead-letter subqueue.

## Run tests

### Backend

```powershell
dotnet test
```

### Frontend

```powershell
cd Frontend
npm test -- --watch=false
```

## Build for production

### Backend

```powershell
dotnet build -c Release
```

### Frontend

```powershell
cd Frontend
npm run build
```

The Angular build output is written to `Frontend/dist/Frontend`.

## Notes

- Deleted tasks use soft deletion and remain in the database with `IsDeleted = 1`.
- Dates are stored and exchanged in UTC/ISO 8601 format.
- Task status values are `Pending`, `InProgress`, `Completed`, `Cancelled`, and `OnHold`.
- Task priority values are `Low`, `Normal`, `High`, and `Critical`.
