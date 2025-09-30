# 🚀 Quick Start Guide

Hướng dẫn nhanh để chạy AuthSystem trong 2 phút!

## Yêu cầu

- ✅ Docker Desktop đã cài đặt và đang chạy
- ✅ Có kết nối internet (để pull Docker images)

## Bước 1: Mở Terminal/Command Prompt

**Windows**: Nhấn `Win + R`, gõ `cmd`, Enter

**Mac/Linux**: Mở Terminal

## Bước 2: Di chuyển đến thư mục project

```bash
cd "D:\TestC#\AuthSystem"
```

## Bước 3: Chạy Docker

### Windows:
```cmd
docker-start.bat
```

### Linux/Mac:
```bash
chmod +x docker-start.sh
./docker-start.sh
```

### Hoặc dùng Docker Compose trực tiếp:
```bash
docker-compose up --build -d
```

## Bước 4: Chờ services khởi động

Quá trình này mất khoảng 1-2 phút:
- Download SQL Server image (nếu chưa có)
- Build API image
- Khởi động containers
- Chạy database migration

## Bước 5: Truy cập ứng dụng

Mở browser và truy cập:

🌐 **Swagger UI**: http://localhost:5000/swagger

## Test API nhanh

### 1. Đăng ký tài khoản

Trong Swagger UI, tìm endpoint `POST /api/auth/register`:

```json
{
  "email": "test@example.com",
  "password": "Test123!@#",
  "confirmPassword": "Test123!@#",
  "firstName": "Test",
  "lastName": "User"
}
```

Nhấn **Execute**

### 2. Đăng nhập

Tìm endpoint `POST /api/auth/login`:

```json
{
  "email": "test@example.com",
  "password": "Test123!@#"
}
```

Nhấn **Execute**

Bạn sẽ nhận được `accessToken` và `refreshToken`

### 3. Test endpoint có authentication

Copy `accessToken` từ bước 2

Nhấn nút **Authorize** (ổ khóa) ở đầu trang Swagger

Nhập: `Bearer {accessToken}` (thay {accessToken} bằng token thực)

Thử endpoint `GET /api/users/me`

## Dừng ứng dụng

### Windows:
```cmd
docker-stop.bat
```

### Linux/Mac:
```bash
./docker-stop.sh
```

### Hoặc:
```bash
docker-compose down
```

## Xem logs (nếu có lỗi)

### Windows:
```cmd
docker-logs.bat
```

### Linux/Mac/Manual:
```bash
docker-compose logs -f
```

## Xóa tất cả (cleanup)

### Windows:
```cmd
docker-clean.bat
```

### Manual:
```bash
docker-compose down -v
docker image prune -f
```

## Xử lý lỗi thường gặp

### ❌ "Docker is not running"
**Giải pháp**: Mở Docker Desktop và đợi nó khởi động

### ❌ "Port 5000 is already allocated"
**Giải pháp**: 
```bash
# Tìm process đang dùng port
netstat -ano | findstr :5000

# Hoặc đổi port trong docker-compose.yml
ports:
  - "5001:80"  # Đổi 5000 -> 5001
```

### ❌ "SQL Server unhealthy"
**Giải pháp**: Chờ thêm 30 giây, SQL Server khởi động chậm

### ❌ "Cannot connect to database"
**Giải pháp**: 
```bash
# Xem logs
docker-compose logs sqlserver

# Restart
docker-compose restart
```

## Endpoints quan trọng

| Endpoint | Method | Mô tả | Auth Required |
|----------|--------|-------|---------------|
| `/api/auth/register` | POST | Đăng ký | ❌ |
| `/api/auth/login` | POST | Đăng nhập | ❌ |
| `/api/auth/verify-2fa` | POST | Xác thực 2FA | ❌ |
| `/api/auth/refresh-token` | POST | Refresh token | ❌ |
| `/api/auth/enable-2fa` | POST | Bật 2FA | ✅ |
| `/api/auth/disable-2fa` | POST | Tắt 2FA | ✅ |
| `/api/users/me` | GET | Thông tin user | ✅ |
| `/api/users` | GET | Danh sách users | ✅ (Admin) |

## Default Credentials

### SQL Server
- **Host**: localhost:1433
- **User**: sa
- **Password**: YourStrong@Passw0rd
- **Database**: AuthSystemDb

### Test User (sau khi register)
Bạn tự tạo qua endpoint `/api/auth/register`

## Next Steps

1. 📖 Đọc [README.md](README.md) để hiểu chi tiết về project
2. 🐳 Đọc [DOCKER.md](DOCKER.md) để hiểu về Docker setup
3. 💻 Xem source code để học Clean Architecture
4. 🔧 Thử các API endpoints khác nhau
5. 🎯 Customize và mở rộng project

## Cần trợ giúp?

- Xem logs: `docker-compose logs -f`
- Check status: `docker-compose ps`
- Restart: `docker-compose restart`
- Full reset: `docker-compose down -v && docker-compose up -d`

## Chúc bạn học tập vui vẻ! 🎉
