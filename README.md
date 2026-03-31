# Microservicio de Turismo — Web API REST

Proyecto académico individual. API REST desarrollada con **ASP.NET Core (.NET 9)** y **Entity Framework Core** para la gestión de un sistema de turismo: tours, usuarios, reservaciones, detalles de reservación y pagos.

---

## Tecnologías

| Tecnología | Versión |
|---|---|
| .NET / ASP.NET Core | 9.0 |
| Entity Framework Core | 9.0.6 |
| SQL Server | (via Docker / cadena de conexión) |
| OpenAPI | 9.0.5 |

---

## Arquitectura

La solución sigue una estructura en capas dentro de un único proyecto:

```
MicroServicioTurismo/
├── Controllers/          # Endpoints HTTP (5 controladores)
├── DTOs/                 # Objetos de transferencia de datos (entrada/salida)
├── Models/               # Entidades del dominio
├── Data/
│   ├── AppDbContext.cs   # Contexto de EF Core
│   └── DbInitializer.cs  # Seed de datos de prueba
└── Utils/
    └── PasswordHelper.cs # Hash/verificación de contraseñas (HMACSHA512)
```

---

## Modelo de datos

```
User ──────< Reservation >────── Tour
                  │
          ┌───────┴────────┐
    ReservationDetail    Payment
```

- **User** — Clientes y empleados. Roles: `Customer`, `Employee`, `Admin`. Contraseña almacenada como hash HMACSHA512 con salt.
- **Tour** — Paquetes turísticos con fecha, capacidad y precio.
- **Reservation** — Vincula un usuario con un tour. Valida disponibilidad de asientos al crear/actualizar.
- **ReservationDetail** — Servicios adicionales asociados a una reservación (ej. guía, almuerzo).
- **Payment** — Registro de pagos asociados a una reservación.

Todos los modelos implementan **borrado lógico** (`IsActive`) además del borrado físico, y campos de auditoría (`CreatedAt`, `UpdatedAt`).

---

## Endpoints

Todos los controladores siguen el patrón `api/[controller]` y exponen las siguientes operaciones:

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/{recurso}` | Listar activos |
| `GET` | `/api/{recurso}/active` | Listar activos (explícito) |
| `GET` | `/api/{recurso}/inactive` | Listar inactivos |
| `GET` | `/api/{recurso}/{id}` | Obtener por ID |
| `POST` | `/api/{recurso}` | Crear |
| `PUT` | `/api/{recurso}/{id}` | Actualizar parcial |
| `DELETE` | `/api/{recurso}/{id}` | Borrado lógico (desactiva) |
| `PATCH` | `/api/{recurso}/{id}/reactivate` | Reactivar |
| `DELETE` | `/api/{recurso}/{id}/physical` | Borrado físico permanente |

**Recursos disponibles:** `tours`, `users`, `reservations`, `reservationdetails`, `payments`

### Validaciones de negocio destacadas

- Al crear una reservación: verifica que el usuario y el tour existan y estén activos, y que haya asientos disponibles.
- Al actualizar una reservación: recalcula la disponibilidad excluyendo la reservación en edición.
- Al crear un usuario: verifica unicidad de `username` y `email`.

---

## Docker

El proyecto incluye un `Dockerfile` multi-stage y un `docker-compose.yml` que levanta dos servicios:

| Servicio | Imagen | Puerto |
|---|---|---|
| `db` | SQL Server 2022 | `1433` |
| `api` | ASP.NET Core 9 (build local) | `8001 → 8080` |

### Levantar con Docker Compose

```bash
cd MicroServicioTurismo
docker compose up --build
```

La API queda disponible en `http://localhost:8001/api/`.

El `docker-compose.yml` inyecta la cadena de conexión como variable de entorno, por lo que no es necesario modificar `appsettings.json`:

```
ConnectionStrings__DefaultConnection=Server=db;Database=ProductDB;User=sa;Password=Server123.;Encrypt=False;TrustServerCertificate=True;
```

> Los contenedores se comunican en la red interna `backend` (bridge). El servicio `api` depende de `db` vía `depends_on`.

### Dockerfile

El build usa dos etapas:
1. **build** — SDK 9.0: restaura, compila y publica en modo Release.
2. **runtime** — ASP.NET 9.0: imagen reducida que solo ejecuta el binario publicado en el puerto `8080`.

---

## Configuración

Al arrancar, la aplicación:
1. Ejecuta `EnsureCreated()` y `Migrate()` para inicializar la base de datos.
2. Llama a `DbInitializer.Initialize()` que inserta datos de prueba si las tablas están vacías (3 usuarios, 2 tours, 2 reservaciones, 2 detalles y 2 pagos).

---

## Ejecución local (sin Docker)

```bash
# Requiere SQL Server accesible y cadena de conexión configurada en appsettings.json
dotnet restore
dotnet run --project MicroServicioTurismo
```

> El CORS está configurado con `AllowAnyOrigin` para entorno de desarrollo.

---

## Estructura del proyecto

```
Web-Api---Microservicio-Turismo/
└── MicroServicioTurismo/
    ├── dockerfile
    ├── docker-compose.yml
    ├── MicroServicioTurismo.csproj
    ├── MicroServicioTurismo.sln
    ├── Program.cs
    ├── appsettings.json
    ├── Controllers/
    │   ├── ToursController.cs
    │   ├── UsersController.cs
    │   ├── ReservationController.cs
    │   ├── ReservationDetailsController.cs
    │   └── PaymentsController.cs
    ├── DTOs/
    │   ├── TourDtos.cs
    │   ├── UserDtos.cs
    │   ├── ReservationDtos.cs
    │   ├── ReservationDetailDtos.cs
    │   └── PaymentDtos.cs
    ├── Models/
    │   ├── Tour.cs
    │   ├── User.cs
    │   ├── Reservation.cs
    │   ├── ReservationDetail.cs
    │   └── Payment.cs
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── DbInitializer.cs
    └── Utils/
        └── PasswordHelper.cs
```

---

## Autor

Proyecto académico desarrollado individualmente.
