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

📄 Leé esto en: [English](README.md) | **Español**

**Proyecto Académico — 2025**
Universidad Privada Domingo Savio — Ing. de Sistemas
Materia: Desarrollo de Aplicaciones Web / Servicios Web

---

## Tabla de Contenidos

- [Descripción](#descripción)
- [Stack](#stack)
- [Arquitectura](#arquitectura)
- [Instalación](#instalación)
- [Endpoints](#endpoints)
- [Datos de Prueba](#datos-de-prueba)
- [Autor](#autor)

---

## Descripción

API REST que gestiona el ciclo completo de un sistema de turismo: alta de tours, registro de usuarios, reservaciones con control de capacidad, detalles adicionales por reserva y pagos.

Características principales:

- CRUD completo para 5 recursos: tours, usuarios, reservaciones, detalles de reservación y pagos
- Borrado lógico (`IsActive`) y físico en todos los recursos, con soporte de reactivación
- Validación de disponibilidad de asientos al crear o actualizar reservaciones
- Hash de contraseñas con HMACSHA512 + salt
- Migraciones automáticas y datos de prueba al iniciar la aplicación
- CORS configurado para entornos de desarrollo

---

## Stack

| Categoría | Tecnología | Versión |
|---|---|---|
| Framework | ASP.NET Core | 9.0 |
| ORM | Entity Framework Core | 9.0.6 |
| Base de datos | SQL Server | 2022 |
| Documentación API | OpenAPI (Swagger) | 9.0.5 |
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

## Instalación

### Opción 1: Docker (Recomendado)

El `docker-compose.yml` levanta dos servicios en una red interna `backend`:

| Servicio | Imagen | Puerto |
|---|---|---|
| `db` | SQL Server 2022 | `1433` |
| `api` | ASP.NET Core 9 (build local) | `8001 → 8080` |

```bash
cd MicroServicioTurismo
docker compose up --build
```

La API queda disponible en `http://localhost:8001/api/`.

> La cadena de conexión se inyecta como variable de entorno desde el compose — no es necesario modificar `appsettings.json`.

### Opción 2: Local (requiere .NET 9 SDK)

1. Clonar el repositorio:
   ```bash
   git clone <url-del-repositorio>
   cd MicroServicioTurismo
   ```

2. Configurar la cadena de conexión en `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=<host>;Database=TurismoDB;User=sa;Password=<contraseña>;Encrypt=False;"
     }
   }
   ```

3. Ejecutar:
   ```bash
   dotnet run
   ```

La aplicación aplica las migraciones e inserta los datos de prueba automáticamente al iniciar.

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
- Reservación: verifica que el usuario y el tour estén activos, y valida la disponibilidad de asientos antes de confirmar.
- Usuario: unicidad de `username` y `email` al crear.

---

## Datos de Prueba

Al iniciar con la base de datos vacía, `DbInitializer` inserta:

| Entidad | Registros |
|---|---|
| Usuarios | 3 (2 Customer, 1 Employee) — contraseña por defecto: `Prueba123` |
| Tours | 2 (recorrido por la ciudad y aventura en la montaña) |
| Reservaciones | 2 (Confirmed y Pending) |
| Detalles | 2 (guía bilingüe, almuerzo opcional) |
| Pagos | 2 (Método: Tarjeta, Estado: Completed) |

---

## Autor

Desarrollado individualmente como proyecto académico.

---

<div align="center">
<img src="https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge" alt="License: MIT">
</div>
