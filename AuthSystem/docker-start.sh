#!/bin/bash

echo "================================"
echo "AuthSystem - Starting Docker..."
echo "================================"

echo ""
echo "Building and starting containers..."
docker-compose up --build -d

echo ""
echo "Waiting for services to be ready..."
sleep 10

echo ""
echo "================================"
echo "Services Status:"
echo "================================"
docker-compose ps

echo ""
echo "================================"
echo "Application URLs:"
echo "================================"
echo "API: http://localhost:5000"
echo "Swagger: http://localhost:5000/swagger"
echo "SQL Server: localhost:1433"
echo "  User: sa"
echo "  Password: YourStrong@Passw0rd"
echo ""
echo "================================"
echo "Useful Commands:"
echo "================================"
echo "View logs: docker-compose logs -f"
echo "Stop: docker-compose down"
echo "Stop and remove volumes: docker-compose down -v"
echo "================================"
