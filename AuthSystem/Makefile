.PHONY: help build up down logs clean restart status

help:
	@echo "AuthSystem Docker Commands"
	@echo "=========================="
	@echo "make build    - Build Docker images"
	@echo "make up       - Start all services"
	@echo "make down     - Stop all services"
	@echo "make logs     - View logs"
	@echo "make restart  - Restart all services"
	@echo "make clean    - Remove all containers and volumes"
	@echo "make status   - Show container status"

build:
	docker-compose build

up:
	docker-compose up -d
	@echo ""
	@echo "Services started!"
	@echo "API: http://localhost:5000"
	@echo "Swagger: http://localhost:5000/swagger"

down:
	docker-compose down

logs:
	docker-compose logs -f

restart:
	docker-compose restart

clean:
	docker-compose down -v
	docker image prune -f

status:
	docker-compose ps
