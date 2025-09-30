# 🎯 START HERE - AuthSystem Pet Project

Chào mừng đến với AuthSystem - Pet project C# về Authentication & Authorization với 2FA!

## 🚀 Bắt đầu nhanh (2 phút)

### Bước 1: Cài đặt Docker Desktop
- Tải về: https://www.docker.com/products/docker-desktop
- Cài đặt và khởi động Docker Desktop

### Bước 2: Chạy project
```bash
# Windows
docker-start.bat

# Linux/Mac
./docker-start.sh
```

### Bước 3: Mở Swagger
Truy cập: http://localhost:5000/swagger

**Done!** 🎉

---

## 📚 Tài liệu

Đọc theo thứ tự để hiểu project tốt nhất:

| Thứ tự | File | Mô tả | Thời gian đọc |
|--------|------|-------|---------------|
| 1️⃣ | [QUICKSTART.md](QUICKSTART.md) | Hướng dẫn chạy và test nhanh | 5 phút |
| 2️⃣ | [README.md](README.md) | Tổng quan về project, features, API | 15 phút |
| 3️⃣ | [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | Cấu trúc code, layers, patterns | 10 phút |
| 4️⃣ | [DOCKER.md](DOCKER.md) | Chi tiết về Docker setup | 10 phút |

---

## 🏗️ Kiến trúc tổng quan

```
┌──────────────────────────────────────────────────┐
│              Client (Browser/Postman)            │
└────────────────────┬─────────────────────────────┘
                     │ HTTP/HTTPS
                     ▼
┌──────────────────────────────────────────────────┐
│         🐳 Docker Container: API                 │
│                                                  │
│  ┌────────────────────────────────────────────┐ │
│  │    API Layer (Controllers)                 │ │
│  └──────────────────┬─────────────────────────┘ │
│                     │                            │
│  ┌──────────────────▼─────────────────────────┐ │
│  │    Application Layer (Business Logic)     │ │
│  └──────────────────┬─────────────────────────┘ │
│                     │                            │
│  ┌──────────────────▼─────────────────────────┐ │
│  │    Infrastructure Layer (Data Access)      │ │
│  └──────────────────┬─────────────────────────┘ │
│                     │                            │
│  ┌──────────────────▼─────────────────────────┐ │
│  │    Domain Layer (Entities, Interfaces)     │ │
│  └────────────────────────────────────────────┘ │
└────────────────────┬─────────────────────────────┘
                     │ Network
                     ▼
┌──────────────────────────────────────────────────┐
│     🐳 Docker Container: SQL Server 2022         │
│         Database: AuthSystemDb                   │
└──────────────────────────────────────────────────┘
```

---

## ✨ Features chính

### 🔐 Authentication
- Register (Đăng ký)
- Login (Đăng nhập)
- JWT Tokens (Access + Refresh)
- Token Refresh
- Logout (Token Revocation)

### 🔒 Two-Factor Authentication (2FA)
- Enable/Disable 2FA
- Email-based 2FA codes
- Code expiration (10 phút)
- Verify 2FA codes

### 👥 Authorization
- **Role-Based Access Control (RBAC)**
  - Admin role
  - User role
- **Permission-Based Authorization**
  - User:Read
  - User:Write
  - User:Delete

### 🛡️ Security
- PBKDF2 password hashing
- JWT bearer authentication
- Refresh token rotation
- IP tracking
- Token expiration

---

## 🎓 Học gì từ project này?

### Backend Development
- ✅ **Clean Architecture** - Tổ chức code chuẩn enterprise
- ✅ **ASP.NET Core Web API** - RESTful API design
- ✅ **Entity Framework Core** - ORM, Migrations, Relationships
- ✅ **JWT Authentication** - Token-based auth
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Unit of Work Pattern** - Transaction management
- ✅ **Dependency Injection** - IoC container

### Security
- ✅ **Password Hashing** - PBKDF2 with salt
- ✅ **Two-Factor Authentication** - 2FA implementation
- ✅ **Role-Based Access Control** - RBAC
- ✅ **Permission-Based Authorization** - Fine-grained access
- ✅ **Token Management** - Refresh tokens, expiration

### DevOps & Docker
- ✅ **Docker** - Containerization
- ✅ **Docker Compose** - Multi-container orchestration
- ✅ **Multi-stage Build** - Optimized Docker images
- ✅ **Health Checks** - Container monitoring
- ✅ **Environment Configuration** - Config management

### Database
- ✅ **SQL Server** - Relational database
- ✅ **Database Migrations** - Schema versioning
- ✅ **Data Seeding** - Initial data
- ✅ **Relationships** - 1:N, N:M relationships
- ✅ **Indexes** - Performance optimization

---

## 📋 Prerequisites

### Để chạy với Docker (Khuyến nghị):
- ✅ Docker Desktop (Windows/Mac) hoặc Docker Engine (Linux)
- ✅ 4GB RAM cho Docker
- ✅ 2GB disk space

### Để chạy trực tiếp:
- ✅ .NET 8.0 SDK
- ✅ SQL Server hoặc LocalDB
- ✅ Visual Studio 2022 / VS Code / Rider

---

## 🔧 Commands hữu ích

### Docker Commands

```bash
# Khởi động
docker-compose up -d

# Xem logs
docker-compose logs -f

# Dừng
docker-compose down

# Xem trạng thái
docker-compose ps

# Rebuild
docker-compose up --build -d

# Clean all
docker-compose down -v
```

### .NET Commands (Nếu chạy local)

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run --project src/AuthSystem.API

# Migrations
dotnet ef migrations add InitialCreate --project src/AuthSystem.Infrastructure --startup-project src/AuthSystem.API
dotnet ef database update --project src/AuthSystem.API
```

---

## 🧪 Test API

### Sử dụng Swagger UI
1. Mở: http://localhost:5000/swagger
2. Thử endpoint `POST /api/auth/register`
3. Thử endpoint `POST /api/auth/login`
4. Copy access token
5. Click "Authorize" button (ổ khóa)
6. Nhập: `Bearer {token}`
7. Thử các endpoints được bảo vệ

### Sử dụng curl

```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!@#",
    "confirmPassword": "Test123!@#",
    "firstName": "Test",
    "lastName": "User"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!@#"
  }'

# Get current user (với token)
curl -X GET http://localhost:5000/api/users/me \
  -H "Authorization: Bearer {YOUR_ACCESS_TOKEN}"
```

---

## 🗂️ Cấu trúc thư mục

```
AuthSystem/
├── 📂 src/                        # Source code
│   ├── AuthSystem.Domain/         # Entities, Interfaces
│   ├── AuthSystem.Application/    # Business Logic
│   ├── AuthSystem.Infrastructure/ # Data Access
│   └── AuthSystem.API/            # Web API
├── 🐳 Dockerfile                  # Docker build
├── 🐳 docker-compose.yml          # Services
├── 📄 README.md                   # Main docs
├── 📄 QUICKSTART.md               # Quick guide
├── 📄 DOCKER.md                   # Docker guide
├── 📄 PROJECT_STRUCTURE.md        # Structure details
└── 📄 START_HERE.md               # This file
```

---

## 🎯 Learning Path

### Beginner (1-2 ngày)
1. ✅ Chạy project với Docker
2. ✅ Test các API endpoints
3. ✅ Đọc README.md
4. ✅ Hiểu flow: Register → Login → Get User

### Intermediate (3-5 ngày)
1. ✅ Đọc toàn bộ source code
2. ✅ Hiểu Clean Architecture
3. ✅ Hiểu JWT authentication flow
4. ✅ Hiểu 2FA implementation
5. ✅ Thử modify và add features

### Advanced (1-2 tuần)
1. ✅ Add Unit Tests
2. ✅ Add Integration Tests
3. ✅ Implement real Email service
4. ✅ Add SMS 2FA
5. ✅ Add Authenticator App 2FA
6. ✅ Deploy to cloud (Azure/AWS)

---

## 🔗 API Endpoints Overview

| Endpoint | Method | Auth | Mô tả |
|----------|--------|------|-------|
| `/api/auth/register` | POST | ❌ | Đăng ký tài khoản |
| `/api/auth/login` | POST | ❌ | Đăng nhập |
| `/api/auth/verify-2fa` | POST | ❌ | Xác thực 2FA |
| `/api/auth/refresh-token` | POST | ❌ | Refresh token |
| `/api/auth/revoke-token` | POST | ✅ | Thu hồi token |
| `/api/auth/enable-2fa` | POST | ✅ | Bật 2FA |
| `/api/auth/disable-2fa` | POST | ✅ | Tắt 2FA |
| `/api/users/me` | GET | ✅ | Thông tin user |
| `/api/users` | GET | ✅ | Danh sách users (Admin) |

---

## 🐛 Troubleshooting

### Docker không chạy
```bash
# Kiểm tra Docker Desktop đang chạy
docker --version

# Kiểm tra containers
docker-compose ps
```

### Port conflict
```bash
# Đổi port trong docker-compose.yml
ports:
  - "5001:80"  # Thay 5000 → 5001
```

### Database error
```bash
# Xem logs
docker-compose logs sqlserver

# Restart
docker-compose restart sqlserver
```

### Full reset
```bash
docker-compose down -v
docker-compose up --build -d
```

---

## 💡 Tips

1. **Đọc code theo thứ tự layers**: Domain → Application → Infrastructure → API
2. **Sử dụng Swagger** để test API thay vì viết code client
3. **Xem logs** khi có lỗi: `docker-compose logs -f`
4. **Thử modify code** để học, Docker sẽ rebuild nhanh
5. **Đặt breakpoints** trong Visual Studio để debug

---

## 📞 Cần giúp đỡ?

1. Đọc [QUICKSTART.md](QUICKSTART.md) - Hướng dẫn từng bước
2. Đọc [DOCKER.md](DOCKER.md) - Troubleshooting Docker
3. Xem logs: `docker-compose logs -f`
4. Check status: `docker-compose ps`

---

## 🎉 Next Steps

Sau khi chạy được project:

1. ✅ Test tất cả API endpoints
2. ✅ Đọc source code từng layer
3. ✅ Thử modify và add features
4. ✅ Add unit tests
5. ✅ Deploy lên cloud

---

## 📝 Notes

- Email service hiện tại chỉ log ra console
- Database data được persist trong Docker volume
- JWT secret trong code CHỈ dùng cho development
- Cần thay đổi config cho production

---

**Happy Coding! 🚀**
