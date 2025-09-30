@echo off
echo ================================
echo AuthSystem - Clean Docker
echo ================================
echo This will remove all containers, volumes and data
echo.
set /p confirm="Are you sure? (y/n): "

if /i "%confirm%"=="y" (
    echo.
    echo Stopping and removing containers...
    docker-compose down -v
    
    echo.
    echo Cleaning up Docker images...
    docker image prune -f
    
    echo.
    echo Clean completed!
) else (
    echo.
    echo Operation cancelled.
)

pause
