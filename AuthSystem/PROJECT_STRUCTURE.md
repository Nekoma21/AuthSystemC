# 📁 Project Structure

## Tổng quan cấu trúc thư mục

```
D:\TestC#\AuthSystem/
│
├── 📂 src/                                    # Source code
│   ├── 📂 AuthSystem.Domain/                  # Domain Layer
│   │   ├── 📂 Entities/                       # Domain entities
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   ├── Permission.cs
│   │   │   ├── UserRole.cs
│   │   │   ├── RolePermission.cs
│   │   │   ├── RefreshToken.cs
│   │   │   └── TwoFactorCode.cs
│   │   ├── 📂 Interfaces/                     # Domain interfaces
│   │   │   ├── IRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── AuthSystem.Domain.csproj
│   │
│   ├── 📂 AuthSystem.Application/             # Application Layer
│   │   ├── 📂 DTOs/
│   │   │   ├── 📂 Auth/                       # Auth DTOs
│   │   │   │   ├── RegisterRequest.cs
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   ├── AuthResponse.cs
│   │   │   │   ├── TwoFactorRequest.cs
│   │   │   │   ├── RefreshTokenRequest.cs
│   │   │   │   └── UserDto.cs
│   │   │   └── 📂 Common/                     # Common DTOs
│   │   │       └── Result.cs
│   │   ├── 📂 Interfaces/                     # Application interfaces
│   │   │   ├── IAuthService.cs
│   │   │   ├── IJwtService.cs
│   │   │   ├── IEmailService.cs
│   │   │   └── IPasswordHasher.cs
│   │   ├── 📂 Services/                       # Business logic
│   │   │   └── AuthService.cs
│   │   └── AuthSystem.Application.csproj
│   │
│   ├── 📂 AuthSystem.Infrastructure/          # Infrastructure Layer
│   │   ├── 📂 Data/                           # Database context
│   │   │   └── ApplicationDbContext.cs
│   │   ├── 📂 Repositories/                   # Data access
│   │   │   ├── Repository.cs
│   │   │   └── UnitOfWork.cs
│   │   ├── 📂 Services/                       # Infrastructure services
│   │   │   ├── JwtService.cs
│   │   │   ├── PasswordHasher.cs
│   │   │   └── EmailService.cs
│   │   └── AuthSystem.Infrastructure.csproj
│   │
│   └── 📂 AuthSystem.API/                     # API Layer
│       ├── 📂 Controllers/                    # API controllers
│       │   ├── AuthController.cs
│       │   └── UsersController.cs
│       ├── 📂 Extensions/                     # Extension methods
│       │   └── DatabaseExtensions.cs
│       ├── Program.cs                         # Application entry point
│       ├── appsettings.json                   # Configuration
│       ├── appsettings.Development.json       # Dev configuration
│       ├── appsettings.Docker.json            # Docker configuration
│       └── AuthSystem.API.csproj
│
├── 🐳 Docker Files/                           # Docker configuration
│   ├── Dockerfile                             # Multi-stage Docker build
│   ├── docker-compose.yml                     # Services orchestration
│   ├── docker-compose.override.yml            # Dev overrides
│   ├── .dockerignore                          # Docker ignore patterns
│   ├── docker-start.bat                       # Windows start script
│   ├── docker-start.sh                        # Linux/Mac start script
│   ├── docker-stop.bat                        # Windows stop script
│   ├── docker-stop.sh                         # Linux/Mac stop script
│   ├── docker-logs.bat                        # Windows logs script
│   ├── docker-clean.bat                       # Windows cleanup script
│   └── Makefile                               # Make commands
│
├── 📄 Documentation/                          # Documentation
│   ├── README.md                              # Main documentation
│   ├── QUICKSTART.md                          # Quick start guide
│   ├── DOCKER.md                              # Docker guide
│   └── PROJECT_STRUCTURE.md                   # This file
│
├── 🔧 Configuration Files/
│   ├── AuthSystem.sln                         # Solution file
│   └── .gitignore                             # Git ignore patterns
│
└── 📦 Runtime (Generated)/
    ├── bin/                                   # Build output
    ├── obj/                                   # Build temporary
    └── Migrations/                            # EF Core migrations
```

## Layers chi tiết

### 1️⃣ Domain Layer (Core)

**Mục đích**: Chứa business logic và entities cốt lõi

**Dependencies**: Không phụ thuộc vào layer nào

**Bao gồm**:
- ✅ Entities: User, Role, Permission, RefreshToken, TwoFactorCode
- ✅ Interfaces: IRepository, IUnitOfWork
- ✅ Enums: TwoFactorCodeType
- ✅ Base classes: BaseEntity

**Không chứa**:
- ❌ Database implementation
- ❌ API controllers
- ❌ External services

### 2️⃣ Application Layer

**Mục đích**: Business logic, use cases, application services

**Dependencies**: Domain Layer

**Bao gồm**:
- ✅ DTOs: Request/Response models
- ✅ Service interfaces: IAuthService, IJwtService, IEmailService
- ✅ Service implementations: AuthService
- ✅ Validators
- ✅ Result wrapper

**Không chứa**:
- ❌ Database code
- ❌ HTTP code
- ❌ UI code

### 3️⃣ Infrastructure Layer

**Mục đích**: External concerns, database, third-party services

**Dependencies**: Domain Layer, Application Layer

**Bao gồm**:
- ✅ DbContext: ApplicationDbContext
- ✅ Repository implementations
- ✅ External services: Email, JWT, Password hashing
- ✅ Migrations
- ✅ Data seeding

**Không chứa**:
- ❌ Business logic
- ❌ API controllers

### 4️⃣ API Layer (Presentation)

**Mục đích**: HTTP endpoints, API configuration

**Dependencies**: Tất cả layers khác

**Bao gồm**:
- ✅ Controllers: AuthController, UsersController
- ✅ Middleware
- ✅ Program.cs configuration
- ✅ API documentation (Swagger)
- ✅ Dependency injection setup

**Không chứa**:
- ❌ Business logic (gọi services)
- ❌ Direct database access (qua services)

## Dependency Flow

```
┌─────────────────────────────────────────┐
│         API Layer                       │
│  (Controllers, Program.cs)              │
└────────────────┬────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────┐
│    Infrastructure Layer                 │
│  (DbContext, Repositories, Services)    │
└────────────────┬────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────┐
│     Application Layer                   │
│  (Business Logic, DTOs, Interfaces)     │
└────────────────┬────────────────────────┘
                 │ depends on
                 ▼
┌─────────────────────────────────────────┐
│      Domain Layer (Core)                │
│  (Entities, Interfaces)                 │
│  ← No dependencies                      │
└─────────────────────────────────────────┘
```

## Key Files và Chức năng

### Domain Layer

| File | Mô tả |
|------|-------|
| `BaseEntity.cs` | Base class cho tất cả entities |
| `User.cs` | User entity với authentication fields |
| `Role.cs` | Role entity cho RBAC |
| `Permission.cs` | Permission entity |
| `RefreshToken.cs` | Refresh token entity |
| `TwoFactorCode.cs` | 2FA code entity |
| `IRepository.cs` | Generic repository interface |
| `IUnitOfWork.cs` | Unit of work pattern interface |

### Application Layer

| File | Mô tả |
|------|-------|
| `AuthService.cs` | Core authentication logic |
| `IAuthService.cs` | Auth service interface |
| `IJwtService.cs` | JWT service interface |
| `IEmailService.cs` | Email service interface |
| `IPasswordHasher.cs` | Password hashing interface |
| `Result.cs` | Generic result wrapper |
| DTOs | Request/Response models |

### Infrastructure Layer

| File | Mô tả |
|------|-------|
| `ApplicationDbContext.cs` | EF Core DbContext với configurations |
| `Repository.cs` | Generic repository implementation |
| `UnitOfWork.cs` | Unit of work implementation |
| `JwtService.cs` | JWT token generation/validation |
| `PasswordHasher.cs` | PBKDF2 password hashing |
| `EmailService.cs` | Email sending (console log) |

### API Layer

| File | Mô tả |
|------|-------|
| `AuthController.cs` | Auth endpoints (register, login, 2FA) |
| `UsersController.cs` | User management endpoints |
| `Program.cs` | Application configuration & DI |
| `DatabaseExtensions.cs` | Auto migration helper |
| `appsettings.json` | Configuration settings |

### Docker Files

| File | Mô tả |
|------|-------|
| `Dockerfile` | Multi-stage build (SDK → Runtime) |
| `docker-compose.yml` | API + SQL Server services |
| `.dockerignore` | Files excluded from build |
| `docker-start.bat/.sh` | Helper scripts |

## Database Schema

### Tables

```sql
-- Users
Users
  - Id (uniqueidentifier, PK)
  - Email (nvarchar(255), unique)
  - PasswordHash (nvarchar(max))
  - FirstName (nvarchar(100))
  - LastName (nvarchar(100))
  - IsEmailVerified (bit)
  - IsTwoFactorEnabled (bit)
  - TwoFactorSecret (nvarchar(max), nullable)
  - IsActive (bit)
  - LastLoginAt (datetime2, nullable)
  - CreatedAt (datetime2)
  - UpdatedAt (datetime2, nullable)

-- Roles
Roles
  - Id (uniqueidentifier, PK)
  - Name (nvarchar(50), unique)
  - Description (nvarchar(500))
  - CreatedAt (datetime2)
  - UpdatedAt (datetime2, nullable)

-- Permissions
Permissions
  - Id (uniqueidentifier, PK)
  - Name (nvarchar(100))
  - Resource (nvarchar(100))
  - Action (nvarchar(50))
  - Description (nvarchar(max))
  - CreatedAt (datetime2)
  - UpdatedAt (datetime2, nullable)

-- UserRoles (Many-to-Many)
UserRoles
  - Id (uniqueidentifier, PK)
  - UserId (FK → Users)
  - RoleId (FK → Roles)
  - CreatedAt (datetime2)

-- RolePermissions (Many-to-Many)
RolePermissions
  - Id (uniqueidentifier, PK)
  - RoleId (FK → Roles)
  - PermissionId (FK → Permissions)
  - CreatedAt (datetime2)

-- RefreshTokens
RefreshTokens
  - Id (uniqueidentifier, PK)
  - UserId (FK → Users)
  - Token (nvarchar(450), unique)
  - ExpiresAt (datetime2)
  - IsRevoked (bit)
  - RevokedAt (datetime2, nullable)
  - RevokedByIp (nvarchar(max), nullable)
  - CreatedByIp (nvarchar(max))
  - CreatedAt (datetime2)

-- TwoFactorCodes
TwoFactorCodes
  - Id (uniqueidentifier, PK)
  - UserId (FK → Users)
  - Code (nvarchar(max))
  - Type (int) -- Email/SMS/Authenticator
  - ExpiresAt (datetime2)
  - IsUsed (bit)
  - UsedAt (datetime2, nullable)
  - CreatedAt (datetime2)
```

### Relationships

```
User ←→ UserRoles ←→ Roles
         ↓
Role ←→ RolePermissions ←→ Permissions

User → RefreshTokens (1:N)
User → TwoFactorCodes (1:N)
```

## Configuration Files

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "LocalDB connection string"
  },
  "Jwt": {
    "Secret": "JWT secret key (min 32 chars)",
    "Issuer": "Token issuer",
    "Audience": "Token audience",
    "ExpirationInHours": "1"
  }
}
```

### docker-compose.yml

Định nghĩa 2 services:
1. **sqlserver**: SQL Server 2022 container
2. **api**: .NET 8.0 API container

## NuGet Packages

### Domain
- Không có dependencies ngoài .NET 8.0

### Application
- `Microsoft.EntityFrameworkCore` (8.0.0)

### Infrastructure
- `Microsoft.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.0)
- `Microsoft.EntityFrameworkCore.Tools` (8.0.0)
- `System.IdentityModel.Tokens.Jwt` (7.0.3)
- `Microsoft.IdentityModel.Tokens` (7.0.3)

### API
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.0)
- `Microsoft.EntityFrameworkCore.Design` (8.0.0)
- `Swashbuckle.AspNetCore` (6.5.0)

## Patterns và Practices

### Design Patterns
- ✅ Clean Architecture
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Dependency Injection
- ✅ Service Layer Pattern
- ✅ DTO Pattern
- ✅ Result Pattern

### Best Practices
- ✅ Separation of Concerns
- ✅ Single Responsibility
- ✅ Interface Segregation
- ✅ Dependency Inversion
- ✅ Async/Await throughout
- ✅ Nullable reference types
- ✅ Using statements for resources
- ✅ CancellationToken support

## Tài nguyên học tập

### Để hiểu Domain Layer
- Đọc: `src/AuthSystem.Domain/Entities/`
- Học về: Domain modeling, Entities, Value objects

### Để hiểu Application Layer
- Đọc: `src/AuthSystem.Application/Services/AuthService.cs`
- Học về: Business logic, Use cases, DTOs

### Để hiểu Infrastructure Layer
- Đọc: `src/AuthSystem.Infrastructure/Data/ApplicationDbContext.cs`
- Học về: EF Core, Repository pattern, Data persistence

### Để hiểu API Layer
- Đọc: `src/AuthSystem.API/Controllers/AuthController.cs`
- Học về: REST APIs, Controllers, HTTP

### Để hiểu Docker
- Đọc: `Dockerfile`, `docker-compose.yml`
- Học về: Containerization, Multi-stage builds, Orchestration
