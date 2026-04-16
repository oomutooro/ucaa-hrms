# UCAA Human Resource Management System (HRMS)

Production-ready starter implementation for Uganda Civil Aviation Authority (UCAA) using:
- Backend: ASP.NET Core Web API (.NET 8)
- Frontend: React + TypeScript
- Database: Microsoft SQL Server + Entity Framework Core
- Auth: Local JWT (modular and ready for future Microsoft Entra ID provider)

## 1. Solution Overview

This repository contains a clean architecture backend and a modern React frontend.

### Backend Layers
- `UCAA.HRMS.Domain`
  - Core entities and enums (`Employee`, `Department`, `LeaveRequest`, `PayrollRecord`, `ShiftAssignment`, `HrDocument`)
- `UCAA.HRMS.Application`
  - DTOs, business services, validation, and abstractions (repositories, auth provider interfaces, storage)
- `UCAA.HRMS.Infrastructure`
  - EF Core DbContext, repositories, ASP.NET Identity, JWT provider, file storage, seed data
- `UCAA.HRMS.API`
  - Controllers, middleware, runtime wiring, auth/cors/swagger setup
- `UCAA.HRMS.Tests`
  - Unit tests for key business logic

### Frontend
- `ucaa-hrms-frontend`
  - React app with JWT auth context, protected routes, enterprise dashboard, and module pages

## 2. Implemented Modules

- Authentication
  - Login endpoint
  - Admin-only register endpoint
  - JWT token issuance
  - ASP.NET Identity password hashing
  - `IAuthProvider` abstraction with `LocalAuthProvider` and placeholder `MicrosoftAuthProvider`

- Employees
  - CRUD-ready API (list/create/update/delete)
  - Unique email and employee ID constraints

- Departments & Hierarchy
  - Nested departments via `ParentDepartmentId`

- RBAC
  - Identity roles: `Admin`, `HR Manager`, `Supervisor`, `Employee`
  - Role-based controller authorization
  - Permission constants included for future fine-grained policy mapping

- Leave Management
  - Apply leave
  - Approve/reject leave
  - Leave balance deduction on approved annual leave
  - Configurable leave policy section in config

- Attendance & Shift Scheduling
  - Assign shift endpoint
  - Rotation generator for `Day -> Night -> Off -> Off`
  - Single slot enforcement per date/shift code

- Payroll
  - Salary + allowance + deduction structure
  - Net pay calculation in DTO projection

- Document Management
  - Upload metadata + file storage abstraction
  - Local storage implementation for now

- Dashboard
  - Total employees
  - Employees on leave
  - Upcoming shifts

## 3. API Endpoints

Base URL: `https://localhost:7044/api` (default local API launch profile)

- `POST /auth/login`
- `POST /auth/register` (Admin role required)
- `GET|POST|PUT|DELETE /employees`
- `GET|POST /departments`
- `GET /leave`
- `POST /leave/apply`
- `POST /leave/{id}/review`
- `GET|POST /payroll`
- `GET /shifts`
- `POST /shifts/assign`
- `POST /shifts/rotation`
- `GET /documents`
- `POST /documents/upload`
- `GET /dashboard/metrics`

## 4. Seed Data

On startup, migration is applied and seed executes:
- Default admin user:
  - Email: `admin@ucaa.go.ug`
  - Password: `Admin@12345`
- Department hierarchy:
  - Engineering
  - Electrical (child)
  - Civil (child)
  - Electronics (child)
- Sample employee record

## 5. Local Run (Step-by-Step)

### Prerequisites
- .NET SDK 8+
- Node.js 20+
- SQL Server 2019+ (or SQL Server Express)

### Backend
1. Update connection string and JWT key in `UCAA.HRMS.API/appsettings.Development.json`.
2. Run API:
   ```bash
   dotnet run --project UCAA.HRMS.API/UCAA.HRMS.API.csproj
   ```

### Frontend
1. Set API URL (optional):
   - Create `ucaa-hrms-frontend/.env` with:
     ```
     REACT_APP_API_BASE_URL=https://localhost:7044/api
     ```
2. Run frontend:
   ```bash
   cd ucaa-hrms-frontend
   npm install
   npm start
   ```

## 6. Build and Test

### Backend build
```bash
dotnet build UCAA.HRMS.slnx
```

### Unit tests
```bash
dotnet test UCAA.HRMS.Tests/UCAA.HRMS.Tests.csproj
```

### Frontend build
```bash
cd ucaa-hrms-frontend
npm run build
```

## 7. Docker Deployment

A full `docker-compose.yml` is included for SQL Server + API + Frontend.

```bash
docker compose up --build
```

Services:
- Frontend: `http://localhost:3000`
- API: `http://localhost:8080`
- SQL Server: `localhost:1433`

## 8. IIS Deployment (Windows)

1. Install prerequisites on the IIS host:
  - IIS with ASP.NET Core Hosting Bundle (.NET 8)
  - SQL Server reachable from the IIS host
2. Publish the API:
  ```bash
  dotnet publish UCAA.HRMS.API/UCAA.HRMS.API.csproj -c Release -o .\\publish\\api
  ```
3. Create an IIS website/app pointing to `.\\publish\\api`.
4. Set app pool to `No Managed Code`.
5. Configure environment variables / `appsettings.Production.json` with SQL Server connection string.
6. Ensure `ConnectionStrings:DefaultConnection` targets SQL Server.
7. Grant the IIS app pool identity access to the upload directory configured in `Storage:RootPath`.
8. Start site and verify `/swagger` or health endpoints.

## 9. Future Entra ID Integration Path

Authentication is intentionally abstracted:
- `IAuthProvider` in Application layer
- `LocalAuthProvider` active implementation
- `MicrosoftAuthProvider` placeholder

To integrate Microsoft Entra ID later:
1. Implement OAuth2/OIDC flow in `MicrosoftAuthProvider`.
2. Add external login endpoint/controller mapping.
3. Keep identity mapping by email as current unique key.
4. Add token validation/issuer settings without changing business controllers.

## 10. Key Configuration

- JWT: `Jwt` section in API settings
- Leave policy: `LeavePolicy` section
- CORS frontend URL: `FrontendUrl`
- File uploads root: `Storage:RootPath`

## 11. Notes

- This implementation is production-oriented starter architecture and intentionally extensible.
- Payroll and permissions are structured for future CBA-driven rule engines and policy-based authorization.
- Local auth only is implemented as requested; Entra integration is prepared but not activated.
