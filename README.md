# 🚀 Employee Management API

> API RESTful completa para gestión de empleados y dependientes con autenticación JWT, implementada con ASP.NET Core, Entity Framework Core, SQL Server y Docker.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)](https://www.docker.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-Educational-green)]()

---

## 📑 Tabla de Contenidos

- [Características Principales](#-características-principales)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación Rápida](#-instalación-rápida)
- [Ejecución del Proyecto](#-ejecución-del-proyecto)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [API Endpoints](#-api-endpoints)
- [Guía de Pruebas con Postman](#-guía-de-pruebas-con-postman)
- [Base de Datos](#-base-de-datos)
- [Migraciones](#-migraciones)
- [Docker](#-docker)
- [Seguridad](#-seguridad)
- [Troubleshooting](#-troubleshooting)
- [Video Demostración](#-video-demostración)

---

## ✨ Características Principales

| Característica | Descripción |
|----------------|-------------|
| 🔐 **Autenticación JWT** | Sistema completo con Access Token y Refresh Token |
| 🏗️ **Repository Pattern** | Implementación con Dependency Injection nativa de .NET |
| 🔄 **CRUD Completo** | Operaciones Create, Read, Update, Delete para todas las entidades |
| 👥 **Gestión de Roles** | Autorización basada en roles (Admin, User) |
| 🗑️ **Soft Delete** | Eliminación lógica de registros |
| 📝 **Campos de Auditoría** | Tracking automático (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy) |
| 🐳 **Docker Ready** | Configuración completa con Docker Compose |
| 📖 **Swagger UI** | Documentación interactiva de la API |
| ✅ **Validaciones** | Data Annotations y validaciones personalizadas |

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework principal
- **Entity Framework Core 8.0** - ORM para acceso a datos
- **SQL Server 2022** - Base de datos relacional
- **JWT (JSON Web Tokens)** - Autenticación y autorización
- **BCrypt.Net** - Hash seguro de contraseñas

### DevOps
- **Docker** - Contenedorización
- **Docker Compose** - Orquestación de servicios

### Documentación
- **Swagger/OpenAPI** - Documentación interactiva de API

---

## 📋 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

1. **[.NET SDK 8.0+](https://dotnet.microsoft.com/download)**
   ```powershell
   dotnet --version  # Verificar instalación
   ```

2. **[Docker Desktop](https://www.docker.com/products/docker-desktop)**
   ```powershell
   docker --version  # Verificar instalación
   ```

3. **[Postman](https://www.postman.com/downloads/)** (para pruebas de API)

4. **[Git](https://git-scm.com/)** (opcional, para clonar el repositorio)

---

## ⚡ Instalación Rápida

### Opción 1: Ejecutar con Docker (Recomendado)

```powershell
# 1. Clonar o descargar el proyecto
git clone <tu-repositorio>
cd EmployeeManagementAPI

# 2. Construir y ejecutar con Docker Compose
docker-compose up --build

# 3. Acceder a la aplicación
# Swagger UI: http://localhost:5000
```

### Opción 2: Ejecución Local (Sin Docker)

```powershell
# 1. Navegar al proyecto
cd EmployeeManagementAPI

# 2. Restaurar dependencias
dotnet restore

# 3. Aplicar migraciones
dotnet ef database update

# 4. Ejecutar la aplicación
dotnet run

# 5. Acceder a: http://localhost:5000
```

---

## 🎯 Ejecución del Proyecto

### Con Docker Compose

```powershell
# Iniciar todos los servicios
docker-compose up

# Iniciar en segundo plano
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down

# Detener y eliminar volúmenes
docker-compose down -v
```

### Sin Docker

```powershell
# Configurar connection string en appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EmployeeManagementDB;Trusted_Connection=True;"
}

# Ejecutar
dotnet run
```

### URLs de Acceso

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Swagger UI** | http://localhost:5000 | Documentación interactiva |
| **Health Check** | http://localhost:5000/health | Estado de la API |
| **API Base** | http://localhost:5000/api | Endpoint base |

---

## 📂 Estructura del Proyecto

```
EmployeeManagementAPI/
│
├── 📁 Controllers/              # Controladores de la API
│   ├── AuthController.cs       # Autenticación (Login, Register, Refresh)
│   ├── DepartmentsController.cs # CRUD de Departamentos
│   ├── EmployeesController.cs   # CRUD de Empleados
│   └── DependentsController.cs  # CRUD de Dependientes
│
├── 📁 Models/                   # Modelos de dominio
│   ├── BaseEntity.cs           # Entidad base con auditoría
│   ├── User.cs                 # Usuario del sistema
│   ├── Department.cs           # Departamento
│   ├── Employee.cs             # Empleado
│   └── Dependent.cs            # Dependiente
│
├── 📁 DTOs/                     # Data Transfer Objects
│   ├── AuthDTOs.cs             # DTOs de autenticación
│   └── DependentDTOs.cs        # DTOs de dependientes
│
├── 📁 Data/                     # Contexto de base de datos
│   └── ApplicationDbContext.cs  # DbContext de EF Core
│
├── 📁 Repositories/             # Patrón Repository
│   ├── Repository.cs           # Repositorio genérico base
│   └── DependentRepository.cs  # Repositorio específico
│
├── 📁 Interfaces/               # Contratos/Interfaces
│   └── IRepository.cs          # Interfaz genérica
│
├── 📁 Services/                 # Servicios de negocio
│   └── JwtService.cs           # Gestión de JWT
│
├── 📄 Program.cs                # Punto de entrada y configuración
├── 📄 appsettings.json          # Configuración de la aplicación
├── 📄 Dockerfile                # Configuración de Docker
└── 📄 docker-compose.yml        # Orquestación de contenedores
```

---

## 🌐 API Endpoints

### 🔐 Autenticación

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `POST` | `/api/auth/register` | Registrar nuevo usuario | ❌ |
| `POST` | `/api/auth/login` | Iniciar sesión | ❌ |
| `POST` | `/api/auth/refresh` | Renovar token | ❌ |
| `POST` | `/api/auth/logout` | Cerrar sesión | ✅ |
| `GET` | `/api/auth/me` | Obtener usuario actual | ✅ |

### 🏢 Departamentos

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/departments` | Listar todos | ✅ |
| `GET` | `/api/departments/{id}` | Obtener por ID | ✅ |
| `GET` | `/api/departments/code/{code}` | Obtener por código | ✅ |
| `POST` | `/api/departments` | Crear departamento | 🔒 Admin |
| `PUT` | `/api/departments/{id}` | Actualizar departamento | 🔒 Admin |
| `DELETE` | `/api/departments/{id}` | Eliminar (soft) | 🔒 Admin |
| `DELETE` | `/api/departments/{id}/permanent` | Eliminar permanentemente | 🔒 Admin |
| `GET` | `/api/departments/search?query=` | Buscar departamentos | ✅ |
| `GET` | `/api/departments/stats` | Estadísticas | ✅ |
| `POST` | `/api/departments/{from}/transfer/{to}` | Transferir empleados | 🔒 Admin |

### 👥 Empleados

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/employees` | Listar todos | ✅ |
| `GET` | `/api/employees/{id}` | Obtener por ID | ✅ |
| `GET` | `/api/employees/department/{id}` | Listar por departamento | ✅ |
| `POST` | `/api/employees` | Crear empleado | ✅ |
| `PUT` | `/api/employees/{id}` | Actualizar empleado | ✅ |
| `DELETE` | `/api/employees/{id}` | Eliminar (soft) | ✅ |
| `DELETE` | `/api/employees/{id}/permanent` | Eliminar permanentemente | 🔒 Admin |
| `GET` | `/api/employees/search?query=` | Buscar empleados | ✅ |
| `GET` | `/api/employees/stats` | Estadísticas | ✅ |

### 👨‍👩‍👧‍👦 Dependientes

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/dependents` | Listar todos | ✅ |
| `GET` | `/api/dependents/{id}` | Obtener por ID | ✅ |
| `GET` | `/api/dependents/employee/{id}` | Listar por empleado | ✅ |
| `POST` | `/api/dependents` | Crear dependiente | ✅ |
| `PUT` | `/api/dependents/{id}` | Actualizar dependiente | ✅ |
| `DELETE` | `/api/dependents/{id}` | Eliminar (soft) | ✅ |
| `DELETE` | `/api/dependents/{id}/permanent` | Eliminar permanentemente | 🔒 Admin |
| `GET` | `/api/dependents/employee/{id}/count` | Contar dependientes | ✅ |

### 🏥 Health Check

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/health` | Estado de la API | ❌ |

**Leyenda:**
- ❌ = Sin autenticación requerida
- ✅ = Requiere autenticación
- 🔒 = Requiere rol Admin

---

## 🧪 Guía de Pruebas con Postman

### Configuración Inicial

#### 1. Crear Entorno en Postman

```
Nombre: Employee Management API
Variables:
- base_url: http://localhost:5000
- token: (se llenará automáticamente)
- refresh_token: (se llenará automáticamente)
```

#### 2. Script Automático para Guardar Token

En la pestaña **Tests** de la petición de Login, agrega:

```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("token", jsonData.token);
    pm.environment.set("refresh_token", jsonData.refreshToken);
    console.log("Token guardado exitosamente");
}
```

### Flujo de Prueba Completo

#### Paso 1️⃣: Login

```http
POST {{base_url}}/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

**Respuesta Esperada:**
```json
{
  "token": "eyJhbGc...",
  "refreshToken": "abc123...",
  "username": "admin",
  "email": "admin@company.com",
  "role": "Admin"
}
```

#### Paso 2️⃣: Crear Departamento

```http
POST {{base_url}}/api/departments
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Ventas",
  "code": "SALES",
  "description": "Departamento de ventas",
  "location": "Edificio C"
}
```

#### Paso 3️⃣: Crear Empleado

```http
POST {{base_url}}/api/employees
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan.perez@company.com",
  "phoneNumber": "555-1234",
  "hireDate": "2025-01-15",
  "position": "Desarrollador",
  "salary": 60000,
  "departmentId": 1
}
```

#### Paso 4️⃣: Crear Dependiente

```http
POST {{base_url}}/api/dependents
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "firstName": "María",
  "lastName": "Pérez",
  "dateOfBirth": "2015-03-10",
  "relationship": "Hija",
  "gender": "Femenino",
  "employeeId": 1
}
```

#### Paso 5️⃣: Listar Todo

```http
GET {{base_url}}/api/departments
Authorization: Bearer {{token}}

GET {{base_url}}/api/employees
Authorization: Bearer {{token}}

GET {{base_url}}/api/dependents
Authorization: Bearer {{token}}
```

### Códigos de Respuesta HTTP

| Código | Significado | Descripción |
|--------|-------------|-------------|
| `200` | ✅ OK | Petición exitosa |
| `201` | ✅ Created | Recurso creado exitosamente |
| `400` | ❌ Bad Request | Error en los datos enviados |
| `401` | ❌ Unauthorized | Token inválido o faltante |
| `403` | ❌ Forbidden | Sin permisos suficientes |
| `404` | ❌ Not Found | Recurso no encontrado |
| `500` | ❌ Server Error | Error interno del servidor |

---

## 🗄️ Base de Datos

### Diagrama de Entidades

```
┌─────────────┐       ┌──────────────┐
│    User     │       │  Department  │
├─────────────┤       ├──────────────┤
│ Id          │       │ Id           │
│ Username    │       │ Name         │
│ Email       │       │ Code         │
│ PasswordHash│       │ Description  │
│ Role        │       │ Location     │
└─────────────┘       └──────────────┘
                             │
                             │ 1:N
                             ▼
                      ┌──────────────┐
                      │   Employee   │
                      ├──────────────┤
                      │ Id           │
                      │ FirstName    │
                      │ LastName     │
                      │ Email        │
                      │ Salary       │
                      │ DepartmentId │
                      └──────────────┘
                             │
                             │ 1:N
                             ▼
                      ┌──────────────┐
                      │  Dependent   │
                      ├──────────────┤
                      │ Id           │
                      │ FirstName    │
                      │ LastName     │
                      │ DateOfBirth  │
                      │ Relationship │
                      │ EmployeeId   │
                      └──────────────┘
```

### Tablas Principales

#### Users
- Usuarios del sistema con autenticación
- Campos: Username, Email, PasswordHash, Role
- Soporte para Refresh Token

#### Departments
- Departamentos de la empresa
- Campos: Name, Code, Description, Location
- Relación 1:N con Employees

#### Employees
- Empleados de la empresa
- Campos: FirstName, LastName, Email, Position, Salary
- Relación N:1 con Department
- Relación 1:N con Dependents

#### Dependents
- Dependientes de empleados
- Campos: FirstName, LastName, DateOfBirth, Relationship
- Relación N:1 con Employee

### Datos Iniciales (Seed)

El sistema crea automáticamente:

**Usuario Administrador:**
- Username: `admin`
- Password: `Admin123!`
- Role: `Admin`

**Departamentos:**
1. Recursos Humanos (HR)
2. Tecnología (IT)

---

## 🔄 Migraciones

### Comandos Principales

```powershell
# Crear nueva migración
dotnet ef migrations add MigrationName

# Aplicar migraciones pendientes
dotnet ef database update

# Listar migraciones
dotnet ef migrations list

# Revertir a migración específica
dotnet ef database update PreviousMigrationName

# Eliminar última migración (no aplicada)
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script

# Revertir todas las migraciones
dotnet ef database update 0
```

### Crear Primera Migración

```powershell
# 1. Eliminar carpeta Migrations si existe
Remove-Item -Recurse -Force Migrations

# 2. Crear migración inicial
dotnet ef migrations add InitialCreate

# 3. Aplicar a la base de datos
dotnet ef database update
```

---

## 🐳 Docker

### Servicios Docker

El proyecto incluye 2 servicios:

1. **SQL Server** - Base de datos en puerto 1433
2. **API** - Aplicación ASP.NET Core en puerto 5000

### Comandos Docker Útiles

```powershell
# Ver contenedores activos
docker ps

# Ver logs de un servicio
docker logs employee_api
docker logs employee_sqlserver

# Reiniciar servicios
docker-compose restart

# Detener todo
docker-compose down

# Limpiar volúmenes
docker-compose down -v

# Reconstruir sin caché
docker-compose build --no-cache

# Ejecutar comandos en contenedor
docker exec -it employee_api bash

# Conectar a SQL Server
docker exec -it employee_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd"
```

### Verificar Estado

```powershell
# Estado de contenedores
docker-compose ps

# Uso de recursos
docker stats

# Espacio en disco
docker system df
```

---

## 🔒 Seguridad

### Características Implementadas

| Característica | Implementación |
|----------------|----------------|
| **Hash de Contraseñas** | BCrypt con costo 11 |
| **JWT Signature** | HMAC-SHA256 |
| **Access Token** | Expira en 60 minutos |
| **Refresh Token** | Expira en 7 días |
| **HTTPS** | Redirection automática |
| **CORS** | Configurado para localhost |
| **Validaciones** | Data Annotations + Custom |

### Consideraciones para Producción

- ⚠️ Cambiar las claves JWT en `appsettings.json`
- ⚠️ Usar certificados SSL válidos
- ⚠️ Configurar CORS solo para dominios permitidos
- ⚠️ Implementar rate limiting
- ⚠️ Usar Azure Key Vault o similar para secrets
- ⚠️ Habilitar logging completo
- ⚠️ Implementar monitoreo y alertas

---

## 🔧 Troubleshooting

### Problema: No puede conectar a SQL Server

```powershell
# Verificar que SQL Server está corriendo
docker ps | Select-String sqlserver

# Ver logs
docker logs employee_sqlserver

# Reiniciar contenedor
docker-compose restart sqlserver

# Esperar 30 segundos después de iniciar SQL Server
Start-Sleep -Seconds 30
```

### Problema: Token JWT Inválido

**Causas comunes:**
- Token expirado (válido por 60 minutos)
- Falta el prefijo "Bearer " en el header
- JWT Key incorrecta en appsettings.json

**Solución:**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Problema: Puerto 5000 ocupado

```powershell
# Cambiar puerto en docker-compose.yml
ports:
  - "5001:5000"

# O en appsettings.json
"Urls": "http://localhost:5001"
```

### Problema: Migraciones no aplicadas

```powershell
# Eliminar base de datos y recrear
docker-compose down -v
docker-compose up --build

# O aplicar manualmente
dotnet ef database update
```

### Problema: Error al construir Docker

```powershell
# Limpiar caché
docker-compose down
docker system prune -a

# Reconstruir sin caché
docker-compose build --no-cache
docker-compose up
```

---

## 🎬 Video Demostración

### Guión Sugerido para el Video

#### 1. Introducción (1-2 min)
- Explicar el proyecto
- Mostrar tecnologías utilizadas
- Mencionar características principales

#### 2. Iniciar el Sistema (2-3 min)
```powershell
docker-compose up
```
- Mostrar logs de SQL Server iniciando
- Mostrar logs de API creando base de datos
- Acceder a Swagger UI

#### 3. Demostración en Postman (10-15 min)

**a) Autenticación**
- Login como admin
- Mostrar token generado
- Explicar Access Token y Refresh Token

**b) CRUD Departamentos**
- Crear nuevo departamento
- Listar departamentos
- Actualizar departamento
- Ver estadísticas

**c) CRUD Empleados**
- Crear empleado asignado a departamento
- Listar empleados con sus departamentos
- Buscar empleado por nombre
- Actualizar salario

**d) CRUD Dependientes**
- Crear dependiente para empleado
- Listar dependientes del empleado
- Actualizar información
- Ver edad calculada automáticamente

**e) Funcionalidades Avanzadas**
- Refresh Token funcionando
- Soft Delete vs Delete Permanente
- Transferir empleados entre departamentos
- Ver campos de auditoría (CreatedAt, UpdatedAt)

#### 4. Base de Datos (3-5 min)
```powershell
docker exec -it employee_sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd"
```

```sql
USE EmployeeManagementDB
GO
SELECT * FROM Users
GO
SELECT * FROM Departments
GO
SELECT * FROM Employees
GO
SELECT * FROM Dependents
GO
```

#### 5. Código (3-5 min)
- Mostrar estructura del proyecto
- Explicar Repository Pattern
- Mostrar campos de auditoría en BaseEntity
- Explicar JWT Service
- Mostrar Dependency Injection en Program.cs

#### 6. Desafíos y Soluciones (2-3 min)
- Mencionar problemas encontrados
- Cómo los solucionaste
- Qué aprendiste

#### 7. Conclusión (1-2 min)
- Recapitular lo demostrado
- Aspectos que más te gustaron
- Posibles mejoras futuras

---

## 📚 Recursos Adicionales

### Documentación Oficial
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT.io](https://jwt.io/)
- [Docker](https://docs.docker.com/)

### Tutoriales Recomendados
- [Repository Pattern en .NET](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)
- [JWT Authentication en ASP.NET Core](https://jasonwatmore.com/post/2021/12/14/net-6-jwt-authentication-tutorial-with-example-api)

---

## 👨‍💻 Autor

Andres Felipe Correa Ramirez
- Video: https://youtu.be/ZPGjUgVC1CI
- GitHub: [@tuusuario](https://github.com/tuusuario)

---

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT para fines educativos.

---

## 🙏 Agradecimientos

- Microsoft por .NET y ASP.NET Core
- Docker por simplificar el despliegue
- La comunidad de desarrolladores por sus contribuciones

---

<div align="center">

**⭐ Si te gustó este proyecto, no olvides darle una estrella ⭐**

**Desarrollado con ❤️ usando ASP.NET Core, Docker, y muchas tazas de café ☕**

</div>
