.PHONY: \
	dev dev-backend dev-frontend \
	prod prod-backend prod-frontend \
	down logs logs-backend logs-frontend rebuild


dev:
	docker compose -f docker-compose.dev.yml up --build

dev-backend:
	docker compose -f docker-compose.dev.yml up --build postgres backend

dev-frontend:
	docker compose -f docker-compose.dev.yml up --build frontend


prod:
	docker compose up --build -d

prod-backend:
	docker compose up --build -d postgres backend

prod-frontend:
	docker compose up --build -d frontend