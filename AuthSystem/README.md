# AuthSystem - Authentication & Authorization with 2FA

Đây là một pet project C# về Authentication và Authorization với Two-Factor Authentication (2FA), được xây dựng theo kiến trúc Clean Architecture.

## Cấu trúc Project

```
AuthSystem/
├── src/
│   ├── AuthSystem.Domain/          # Domain Layer - Entities, Interfaces
│   ├── AuthSystem.Application/     # Application Layer - Business Logic, DTOs, Services
│   ├── AuthSystem.Infrastructure/  # Infrastructure Layer - Database, External Services
│   └── AuthSystem.API/            # API Layer - Controllers, Middleware
└── AuthSystem.sln                 # Solution file
```

## Công nghệ sử dụng

- **.NET 8.0**: Framework chính
- **Entity Framework Core**: ORM cho database
- **SQL Server**: Database (LocalDB cho development)
- **JWT (JSON Web Tokens)**: Authentication
- **Swagger/OpenAPI**: API Documentation
- **Clean Architecture**: Pattern tổ chức code

## Tính năng chính

### 1. Authentication
- ✅ Đăng ký tài khoản (Register)
- ✅ Đăng nhập (Login)
- ✅ JWT Access Token & Refresh Token
- ✅ Token refresh mechanism
- ✅ Token revocation

### 2. Two-Factor Authentication (2FA)
- ✅ Bật/tắt 2FA cho user
- ✅ Gửi mã xác thực qua email
- ✅ Xác thực 2FA code
- ✅ Thời gian hết hạn cho mã 2FA (10 phút)

### 3. Authorization
- ✅ Role-Based Access Control (RBAC)
- ✅ Permission-Based Authorization
- ✅ Roles mặc định: Admin, User
- ✅ Permissions tùy chỉnh

### 4. Security
- ✅ Password hashing với PBKDF2
- ✅ Refresh token rotation
- ✅ IP tracking cho tokens
- ✅ Token expiration & validation

## Cài đặt và Chạy Project

### 🐳 Option 1: Chạy với Docker (Khuyến nghị - Dễ nhất!)

#### Yêu cầu
- Docker Desktop
- Docker Compose

#### Các bước thực hiện

**Windows:**
```bash
# Chạy project
docker-start.bat

# Xem logs
docker-logs.bat

# Dừng project
docker-stop.bat

# Xóa tất cả containers và volumes
docker-clean.bat
```

**Linux/Mac:**
```bash
# Chạy project
./docker-start.sh

# Xem logs
docker-compose logs -f

# Dừng project
./docker-stop.sh
```

**Hoặc sử dụng docker-compose trực tiếp:**
```bash
# Build và khởi động
docker-compose up --build -d

# Xem logs
docker-compose logs -f

# Dừng
docker-compose down

# Dừng và xóa volumes
docker-compose down -v
```

**Hoặc sử dụng Makefile (nếu có make):**
```bash
make up      # Khởi động services
make logs    # Xem logs
make down    # Dừng services
make clean   # Xóa tất cả
make status  # Xem trạng thái
```

Sau khi chạy, truy cập:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433
  - User: `sa`
  - Password: `YourStrong@Passw0rd`

> **Lưu ý**: Database sẽ tự động được tạo và migrate khi container khởi động lần đầu!

---

### 💻 Option 2: Chạy trực tiếp với .NET

#### Yêu cầu
- .NET 8.0 SDK
- SQL Server hoặc SQL Server LocalDB
- Visual Studio 2022 hoặc VS Code

#### Các bước thực hiện

1. **Clone/Download project này**

2. **Khôi phục packages**
```bash
cd D:\TestC#\AuthSystem
dotnet restore
```

3. **Cập nhật Connection String** (nếu cần)
Mở file `src/AuthSystem.API/appsettings.json` và cập nhật connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuthSystemDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

4. **Tạo Database và chạy Migrations**
```bash
cd src/AuthSystem.API
dotnet ef migrations add InitialCreate --project ../AuthSystem.Infrastructure
dotnet ef database update
```

5. **Chạy API**
```bash
dotnet run --project src/AuthSystem.API
```

API sẽ chạy tại:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Authentication Endpoints

#### 1. Register (Đăng ký)
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

#### 2. Login (Đăng nhập)
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Response (nếu không có 2FA):
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "xyz123...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "requiresTwoFactor": false,
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"],
    "permissions": []
  }
}
```

Response (nếu có 2FA enabled):
```json
{
  "requiresTwoFactor": true
}
```

#### 3. Verify 2FA Code
```http
POST /api/auth/verify-2fa
Content-Type: application/json

{
  "email": "user@example.com",
  "code": "123456"
}
```

#### 4. Refresh Token
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "xyz123..."
}
```

#### 5. Enable 2FA
```http
POST /api/auth/enable-2fa
Authorization: Bearer {access_token}
```

#### 6. Disable 2FA
```http
POST /api/auth/disable-2fa
Authorization: Bearer {access_token}
```

#### 7. Revoke Token
```http
POST /api/auth/revoke-token
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "refreshToken": "xyz123..."
}
```

### User Endpoints

#### 1. Get Current User
```http
GET /api/users/me
Authorization: Bearer {access_token}
```

#### 2. Get All Users (Admin only)
```http
GET /api/users
Authorization: Bearer {access_token}
```

## Kiến trúc Clean Architecture

### 1. Domain Layer (`AuthSystem.Domain`)
- **Entities**: User, Role, Permission, RefreshToken, TwoFactorCode
- **Interfaces**: IRepository, IUnitOfWork
- Không phụ thuộc vào layer nào khác

### 2. Application Layer (`AuthSystem.Application`)
- **DTOs**: Request/Response models
- **Interfaces**: IAuthService, IJwtService, IEmailService
- **Services**: AuthService (business logic)
- Phụ thuộc vào Domain Layer

### 3. Infrastructure Layer (`AuthSystem.Infrastructure`)
- **Data**: ApplicationDbContext, Migrations
- **Repositories**: Repository pattern implementation
- **Services**: JwtService, PasswordHasher, EmailService
- Phụ thuộc vào Domain và Application

### 4. API Layer (`AuthSystem.API`)
- **Controllers**: AuthController, UsersController
- **Configuration**: Program.cs, Startup
- Phụ thuộc vào tất cả các layer khác

## Database Schema

### Users Table
- Id (Guid, PK)
- Email (string, unique)
- PasswordHash (string)
- FirstName, LastName
- IsEmailVerified (bool)
- IsTwoFactorEnabled (bool)
- TwoFactorSecret (string?)
- IsActive (bool)
- LastLoginAt (DateTime?)
- CreatedAt, UpdatedAt

### Roles Table
- Id (Guid, PK)
- Name (string, unique)
- Description (string)

### Permissions Table
- Id (Guid, PK)
- Name (string)
- Resource (string)
- Action (string)
- Description (string)

### UserRoles (Many-to-Many)
### RolePermissions (Many-to-Many)
### RefreshTokens (One-to-Many với User)
### TwoFactorCodes (One-to-Many với User)

## 🐳 Docker Configuration

Project này đã được dockerized hoàn toàn với:

### Services
1. **API Container**: .NET 8.0 Web API
2. **SQL Server Container**: Microsoft SQL Server 2022

### Docker Files
- `Dockerfile`: Multi-stage build cho API
- `docker-compose.yml`: Orchestration cho tất cả services
- `docker-compose.override.yml`: Development overrides
- `.dockerignore`: Loại trừ files không cần thiết

### Features
- ✅ Multi-stage Docker build (giảm image size)
- ✅ Auto database migration khi startup
- ✅ Health checks cho SQL Server
- ✅ Persistent volumes cho database
- ✅ Network isolation giữa containers
- ✅ Environment variables configuration
- ✅ Helper scripts (Windows & Linux)

### Kiến trúc Docker

```
┌─────────────────────────────────────┐
│   Docker Host (localhost)           │
│                                     │
│  ┌────────────────────────────┐    │
│  │  API Container             │    │
│  │  Port: 5000 -> 80          │    │
│  │  .NET 8.0 Runtime          │    │
│  └────────────┬───────────────┘    │
│               │                     │
│               │ authsystem-network  │
│               │                     │
│  ┌────────────▼───────────────┐    │
│  │  SQL Server Container      │    │
│  │  Port: 1433 -> 1433        │    │
│  │  Volume: sqlserver-data    │    │
│  └────────────────────────────┘    │
└─────────────────────────────────────┘
```

## Học gì từ project này?

### Backend & Architecture
1. **Clean Architecture**: Tổ chức code theo layers, separation of concerns
2. **Entity Framework Core**: ORM, Migrations, Relationships
3. **JWT Authentication**: Access token, Refresh token
4. **Two-Factor Authentication**: 2FA implementation
5. **Role-Based & Permission-Based Authorization**
6. **Repository Pattern & Unit of Work**
7. **Dependency Injection**
8. **RESTful API Design**
9. **Security Best Practices**: Password hashing, token rotation
10. **Async/Await pattern**

### DevOps & Containerization
11. **Docker**: Containerization, Multi-stage builds
12. **Docker Compose**: Service orchestration
13. **Database Migration**: Automatic migration in containers
14. **Health Checks**: Container health monitoring
15. **Environment Configuration**: Managing configs across environments

## Mở rộng Project

Bạn có thể mở rộng project bằng cách:

1. Thêm Email Service thực sự (SMTP, SendGrid)
2. Thêm SMS 2FA
3. Thêm Authenticator App 2FA (Google Authenticator)
4. Thêm Social Login (Google, Facebook)
5. Thêm Rate Limiting
6. Thêm Audit Logging
7. Thêm Unit Tests & Integration Tests
8. Thêm API Versioning
9. Thêm Password Reset functionality
10. Thêm Email Verification

## Lưu ý

- Email service hiện tại chỉ log ra console (không gửi email thực)
- JWT Secret trong appsettings.json cần thay đổi trong production
- Connection string cần cập nhật cho môi trường production
- Cần implement proper error handling và validation

## License

Project này dùng cho mục đích học tập.
