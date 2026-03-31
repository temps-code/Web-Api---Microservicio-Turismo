<div align="center">

<h1>Microservicio de Turismo</h1>

<p><strong>API REST para la gestión de tours, reservaciones y pagos — desarrollada con ASP.NET Core 9 y dockerizada con SQL Server.</strong></p>

<p>
  <img src="https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white" alt="Docker">
</p>

</div>

---

**Proyecto Académico — 2025**
Materia: Desarrollo de Aplicaciones Web / Servicios Web

---

## Descripción

API REST que gestiona el ciclo completo de un sistema de turismo: alta de tours, registro de usuarios, reservaciones con control de capacidad, detalles adicionales por reserva y pagos.

Todos los recursos implementan **borrado lógico** (`IsActive`) además del borrado físico, y campos de auditoría (`CreatedAt`, `UpdatedAt`). Al iniciar, la aplicación aplica migraciones automáticamente e inserta datos de prueba si la base está vacía.

---

## Stack

| Categoría | Tecnología | Versión |
|---|---|---|
| Framework | ASP.NET Core | 9.0 |
| ORM | Entity Framework Core | 9.0.6 |
| Base de datos | SQL Server | 2022 (Docker) |
| Documentación | OpenAPI (Swagger) | 9.0.5 |
| Contenedores | Docker + Docker Compose | — |

---

## Arquitectura

```
MicroServicioTurismo/
├── Controllers/          # Endpoints HTTP (5 controladores)
├── DTOs/                 # Objetos de transferencia de datos
├── Models/               # Entidades del dominio
├── Data/
│   ├── AppDbContext.cs   # Contexto de EF Core
│   └── DbInitializer.cs  # Seed de datos de prueba
├── Utils/
│   └── PasswordHelper.cs # Hash HMACSHA512 con salt
├── dockerfile
└── docker-compose.yml
```

**Modelo de datos:**

```
User ──────< Reservation >────── Tour
                  │
          ┌───────┴────────┐
    ReservationDetail    Payment
```

---

## Docker

El `docker-compose.yml` levanta dos servicios conectados en una red interna `backend`:

| Servicio | Imagen | Puerto expuesto |
|---|---|---|
| `db` | SQL Server 2022 | `1433` |
| `api` | ASP.NET Core 9 (build local) | `8001 → 8080` |

```bash
cd MicroServicioTurismo
docker compose up --build
```

La API queda disponible en `http://localhost:8001/api/`.

> La cadena de conexión se inyecta como variable de entorno desde el compose — no es necesario modificar `appsettings.json`.

---

## Instalación local (sin Docker)

Requiere .NET 9 SDK y una instancia de SQL Server accesible.

1. Clonar el repositorio:
   ```bash
   git clone <url-del-repo>
   cd MicroServicioTurismo
   ```

2. Configurar la cadena de conexión en `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=<host>;Database=TurismoDB;User=sa;Password=<password>;Encrypt=False;"
     }
   }
   ```

3. Ejecutar:
   ```bash
   dotnet run
   ```

La aplicación aplica las migraciones e inserta el seed de prueba automáticamente al iniciar.

---

## Endpoints

Todos los controladores siguen el patrón `api/[controller]` y exponen el mismo conjunto de operaciones:

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/{recurso}` | Listar registros activos |
| `GET` | `/api/{recurso}/active` | Listar activos (explícito) |
| `GET` | `/api/{recurso}/inactive` | Listar inactivos |
| `GET` | `/api/{recurso}/{id}` | Obtener por ID |
| `POST` | `/api/{recurso}` | Crear |
| `PUT` | `/api/{recurso}/{id}` | Actualizar |
| `DELETE` | `/api/{recurso}/{id}` | Borrado lógico |
| `PATCH` | `/api/{recurso}/{id}/reactivate` | Reactivar |
| `DELETE` | `/api/{recurso}/{id}/physical` | Borrado físico permanente |

**Recursos:** `tours` · `users` · `reservations` · `reservationdetails` · `payments`

**Validaciones de negocio:**
- Reservación: verifica usuario y tour activos, y disponibilidad de asientos antes de confirmar.
- Usuario: unicidad de `username` y `email` al crear.

---

## Datos de prueba (Seed)

Al iniciar con la base vacía, `DbInitializer` inserta:

| Entidad | Registros |
|---|---|
| Usuarios | 3 (2 Customer, 1 Employee) — password: `Prueba123` |
| Tours | 2 (ciudad y montaña) |
| Reservaciones | 2 (Confirmed y Pending) |
| Detalles | 2 (guía bilingüe, almuerzo opcional) |
| Pagos | 2 (método: Tarjeta, estado: Completed) |

---

## Autor

Desarrollado individualmente como proyecto académico.

---

<div align="center">
<img src="https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge" alt="License: MIT">
</div>
