# 🐳 Docker Guide - AuthSystem

Hướng dẫn chi tiết về việc chạy AuthSystem với Docker.

## Yêu cầu

- **Docker Desktop** (Windows/Mac) hoặc **Docker Engine** (Linux)
- **Docker Compose** (thường đi kèm với Docker Desktop)
- Ít nhất **4GB RAM** dành cho Docker
- Khoảng **2GB disk space**

## Cấu trúc Docker Files

```
AuthSystem/
├── Dockerfile                    # Multi-stage build cho API
├── docker-compose.yml           # Service orchestration
├── docker-compose.override.yml  # Development settings
├── .dockerignore               # Files bỏ qua khi build
├── docker-start.bat/.sh        # Script khởi động (Windows/Linux)
├── docker-stop.bat/.sh         # Script dừng
├── docker-logs.bat             # Xem logs
├── docker-clean.bat            # Cleanup
└── Makefile                    # Make commands
```

## Cách chạy

### Windows

#### Cách 1: Sử dụng Batch Scripts (Đơn giản nhất)

```cmd
# Khởi động tất cả services
docker-start.bat

# Xem logs real-time
docker-logs.bat

# Dừng services
docker-stop.bat

# Cleanup (xóa containers, volumes, images)
docker-clean.bat
```

#### Cách 2: Sử dụng Docker Compose trực tiếp

```cmd
# Build và khởi động
docker-compose up --build -d

# Xem logs
docker-compose logs -f api

# Xem logs của SQL Server
docker-compose logs -f sqlserver

# Dừng
docker-compose down

# Dừng và xóa volumes (data sẽ mất!)
docker-compose down -v
```

### Linux/Mac

#### Cách 1: Sử dụng Shell Scripts

```bash
# Cấp quyền thực thi (lần đầu tiên)
chmod +x docker-start.sh docker-stop.sh

# Khởi động
./docker-start.sh

# Dừng
./docker-stop.sh
```

#### Cách 2: Sử dụng Makefile

```bash
# Xem các commands có sẵn
make help

# Khởi động
make up

# Xem logs
make logs

# Xem trạng thái
make status

# Dừng
make down

# Cleanup
make clean

# Restart
make restart
```

## Services

### 1. SQL Server Container

**Container Name**: `authsystem-sqlserver`

**Image**: `mcr.microsoft.com/mssql/server:2022-latest`

**Port**: `1433:1433`

**Credentials**:
- Username: `sa`
- Password: `YourStrong@Passw0rd`

**Volume**: `sqlserver-data` (persistent storage)

**Health Check**: Kiểm tra mỗi 10 giây

### 2. API Container

**Container Name**: `authsystem-api`

**Base Image**: 
- Build: `mcr.microsoft.com/dotnet/sdk:8.0`
- Runtime: `mcr.microsoft.com/dotnet/aspnet:8.0`

**Port**: `5000:80`

**Environment Variables**:
```yaml
ASPNETCORE_ENVIRONMENT: Development
ASPNETCORE_URLS: http://+:80
ConnectionStrings__DefaultConnection: Server=sqlserver,1433;Database=AuthSystemDb;...
Jwt__Secret: YourSuperSecretKeyThatIsAtLeast32CharactersLong!
Jwt__Issuer: AuthSystem
Jwt__Audience: AuthSystemUsers
Jwt__ExpirationInHours: 1
```

## Truy cập Services

Sau khi containers chạy thành công:

| Service | URL | Credentials |
|---------|-----|-------------|
| API | http://localhost:5000 | - |
| Swagger UI | http://localhost:5000/swagger | - |
| SQL Server | localhost:1433 | sa / YourStrong@Passw0rd |

## Database Migration

Database sẽ **tự động được tạo và migrate** khi API container khởi động lần đầu tiên.

### Cơ chế hoạt động:

1. API container khởi động
2. Chờ SQL Server healthy (health check)
3. Chạy `InitializeDatabaseAsync()` trong `Program.cs`
4. Tự động apply pending migrations
5. Seed initial data (Roles, Permissions)
6. API sẵn sàng nhận requests

### Xem migration logs:

```bash
docker-compose logs api | grep -i "migration"
```

## Kiểm tra trạng thái

### Xem containers đang chạy

```bash
docker-compose ps
```

Output mong đợi:
```
NAME                   STATUS         PORTS
authsystem-api         Up 2 minutes   0.0.0.0:5000->80/tcp
authsystem-sqlserver   Up 2 minutes   0.0.0.0:1433->1433/tcp
```

### Xem logs

```bash
# Tất cả services
docker-compose logs -f

# Chỉ API
docker-compose logs -f api

# Chỉ SQL Server
docker-compose logs -f sqlserver

# 100 dòng cuối
docker-compose logs --tail=100
```

### Kiểm tra health

```bash
# Health check của SQL Server
docker inspect authsystem-sqlserver --format='{{.State.Health.Status}}'

# Nên trả về: healthy
```

## Kết nối vào container

### Vào API container

```bash
docker exec -it authsystem-api bash
```

### Vào SQL Server container

```bash
docker exec -it authsystem-sqlserver bash
```

### Kết nối SQL Server bằng sqlcmd

```bash
docker exec -it authsystem-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd
```

Sau đó chạy SQL queries:
```sql
-- Xem databases
SELECT name FROM sys.databases;
GO

-- Sử dụng AuthSystemDb
USE AuthSystemDb;
GO

-- Xem tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
GO

-- Đếm users
SELECT COUNT(*) FROM Users;
GO
```

## Troubleshooting

### Problem 1: Container không khởi động

**Kiểm tra logs:**
```bash
docker-compose logs api
docker-compose logs sqlserver
```

**Solution:**
- Đảm bảo ports 5000 và 1433 không bị sử dụng
- Kiểm tra Docker Desktop có đang chạy
- Thử rebuild: `docker-compose up --build --force-recreate`

### Problem 2: SQL Server không healthy

**Kiểm tra:**
```bash
docker logs authsystem-sqlserver
```

**Solution:**
- Chờ thêm 30 giây (SQL Server khởi động chậm)
- Kiểm tra RAM đủ (cần ít nhất 2GB)
- Restart: `docker-compose restart sqlserver`

### Problem 3: Database migration failed

**Kiểm tra logs:**
```bash
docker-compose logs api | grep -i "error\|exception"
```

**Solution:**
- Xóa và tạo lại: `docker-compose down -v && docker-compose up -d`
- Kiểm tra connection string trong docker-compose.yml
- Đảm bảo SQL Server đã healthy trước khi API start

### Problem 4: Port conflict

**Error:** `Bind for 0.0.0.0:5000 failed: port is already allocated`

**Solution:**
```bash
# Tìm process đang dùng port
# Windows
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000

# Đổi port trong docker-compose.yml
ports:
  - "5001:80"  # Thay 5000 thành 5001
```

### Problem 5: Slow build

**Solution:**
```bash
# Clear build cache
docker builder prune -a

# Build không cache
docker-compose build --no-cache
```

## Development Tips

### 1. Watch logs real-time

```bash
# Tất cả logs
docker-compose logs -f

# Chỉ API với tail
docker-compose logs -f --tail=50 api
```

### 2. Restart chỉ API

```bash
docker-compose restart api
```

### 3. Rebuild chỉ API

```bash
docker-compose up -d --build api
```

### 4. Backup database

```bash
# Backup
docker exec authsystem-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -Q "BACKUP DATABASE AuthSystemDb TO DISK='/var/opt/mssql/backup/authsystem.bak'"

# Copy backup ra host
docker cp authsystem-sqlserver:/var/opt/mssql/backup/authsystem.bak ./backup/
```

### 5. Restore database

```bash
# Copy backup vào container
docker cp ./backup/authsystem.bak authsystem-sqlserver:/var/opt/mssql/backup/

# Restore
docker exec authsystem-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -Q "RESTORE DATABASE AuthSystemDb FROM DISK='/var/opt/mssql/backup/authsystem.bak' WITH REPLACE"
```

## Clean Up

### Dừng và xóa containers

```bash
docker-compose down
```

### Xóa containers và volumes (DATA SẼ MẤT!)

```bash
docker-compose down -v
```

### Xóa images

```bash
docker-compose down --rmi all
```

### Full cleanup

```bash
# Windows
docker-clean.bat

# Linux/Mac hoặc Manual
docker-compose down -v
docker image prune -a -f
docker volume prune -f
```

## Production Considerations

Khi deploy lên production, cần thay đổi:

### 1. Security
- Đổi SQL Server password mạnh hơn
- Đổi JWT Secret key
- Tắt Swagger trong production
- Sử dụng HTTPS

### 2. Configuration
```yaml
# docker-compose.prod.yml
api:
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - ASPNETCORE_URLS=https://+:443;http://+:80
  volumes:
    - ./certs:/app/certs
```

### 3. Database
- Sử dụng managed database service (Azure SQL, AWS RDS)
- Backup strategy
- High availability setup

### 4. Monitoring
- Add health check endpoints
- Logging aggregation (ELK, Seq)
- Metrics (Prometheus, Grafana)

## Advanced

### Multi-environment setup

```bash
# Development
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up

# Production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up
```

### Scale API container

```bash
docker-compose up -d --scale api=3
```

### Custom network

```bash
# Tạo network
docker network create authsystem-net

# Sử dụng trong docker-compose.yml
networks:
  default:
    external: true
    name: authsystem-net
```

## Tài liệu tham khảo

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [SQL Server Docker](https://hub.docker.com/_/microsoft-mssql-server)

## Hỗ trợ

Nếu gặp vấn đề:
1. Kiểm tra logs: `docker-compose logs`
2. Kiểm tra status: `docker-compose ps`
3. Recreate containers: `docker-compose up -d --force-recreate`
4. Full reset: `docker-compose down -v && docker-compose up -d`
