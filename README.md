<div align="center">

<h1>Tourism Microservice</h1>

<p><strong>REST API for managing tours, reservations, and payments — built with ASP.NET Core 9 and dockerized with SQL Server.</strong></p>

<p>
  <img src="https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white" alt="Docker">
</p>

</div>

---

📄 Read this in: **English** | [Español](README.es.md)

**Academic Project — 2025**
Universidad Privada Domingo Savio — Ing. de Sistemas
Course: Web Application Development / Web Services

---

## Table of Contents

- [What It Does](#what-it-does)
- [Stack](#stack)
- [Architecture](#architecture)
- [Installation](#installation)
- [Endpoints](#endpoints)
- [Seed Data](#seed-data)
- [Author](#author)

---

## What It Does

REST API that manages the complete lifecycle of a tourism system: tour management, user registration, reservations with capacity control, additional reservation details, and payments.

Key features:

- Full CRUD for 5 resources: tours, users, reservations, reservation details, and payments
- Logical delete (`IsActive`) and physical delete on all resources, with reactivation support
- Capacity validation when creating or updating reservations
- Password hashing with HMACSHA512 + salt
- Automatic database migration and seed data on startup
- CORS configured for development environments

---

## Stack

| Category | Technology | Version |
|---|---|---|
| Framework | ASP.NET Core | 9.0 |
| ORM | Entity Framework Core | 9.0.6 |
| Database | SQL Server | 2022 |
| API Docs | OpenAPI (Swagger) | 9.0.5 |
| Containers | Docker + Docker Compose | — |

---

## Architecture

```
MicroServicioTurismo/
├── Controllers/          # HTTP endpoints (5 controllers)
├── DTOs/                 # Data transfer objects
├── Models/               # Domain entities
├── Data/
│   ├── AppDbContext.cs   # EF Core context
│   └── DbInitializer.cs  # Test data seed
├── Utils/
│   └── PasswordHelper.cs # HMACSHA512 hashing with salt
├── dockerfile
└── docker-compose.yml
```

**Data model:**

```
User ──────< Reservation >────── Tour
                  │
          ┌───────┴────────┐
    ReservationDetail    Payment
```

---

## Installation

### Option 1: Docker (Recommended)

The `docker-compose.yml` spins up two services on an internal `backend` network:

| Service | Image | Port |
|---|---|---|
| `db` | SQL Server 2022 | `1433` |
| `api` | ASP.NET Core 9 (local build) | `8001 → 8080` |

```bash
cd MicroServicioTurismo
docker compose up --build
```

The API will be available at `http://localhost:8001/api/`.

> The connection string is injected as an environment variable from the compose file — no changes to `appsettings.json` required.

### Option 2: Local (.NET 9 SDK required)

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd MicroServicioTurismo
   ```

2. Configure the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=<host>;Database=TurismoDB;User=sa;Password=<password>;Encrypt=False;"
     }
   }
   ```

3. Run:
   ```bash
   dotnet run
   ```

The application automatically applies migrations and inserts seed data on startup.

---

## Endpoints

All controllers follow the `api/[controller]` pattern and expose the same set of operations:

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/{resource}` | List active records |
| `GET` | `/api/{resource}/active` | List active (explicit) |
| `GET` | `/api/{resource}/inactive` | List inactive |
| `GET` | `/api/{resource}/{id}` | Get by ID |
| `POST` | `/api/{resource}` | Create |
| `PUT` | `/api/{resource}/{id}` | Update |
| `DELETE` | `/api/{resource}/{id}` | Logical delete |
| `PATCH` | `/api/{resource}/{id}/reactivate` | Reactivate |
| `DELETE` | `/api/{resource}/{id}/physical` | Permanent delete |

**Resources:** `tours` · `users` · `reservations` · `reservationdetails` · `payments`

**Business rules:**
- Reservation: validates that user and tour are active, and checks seat availability before confirming.
- User: enforces unique `username` and `email` on creation.

---

## Seed Data

On first startup with an empty database, `DbInitializer` inserts:

| Entity | Records |
|---|---|
| Users | 3 (2 Customer, 1 Employee) — default password: `Prueba123` |
| Tours | 2 (city tour and mountain hike) |
| Reservations | 2 (Confirmed and Pending) |
| Details | 2 (bilingual guide, optional lunch) |
| Payments | 2 (Method: Card, Status: Completed) |

---

## Author

Developed individually as an academic project.

---

<div align="center">
<img src="https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge" alt="License: MIT">
</div>
