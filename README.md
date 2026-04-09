# 🐦 XClone Backend - Red Social API

![License](https://img.shields.io/badge/license-MIT-blue)
![Status](https://img.shields.io/badge/status-In%20Development-yellow)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)

> Una API REST completa para un clon de red social (similar a Twitter/X), construida con arquitectura limpia y mejores prácticas.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Stack Tecnológico](#-stack-tecnológico)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Quick Start](#-quick-start)
- [Configuración](#-configuración)
- [API Endpoints](#-api-endpoints)
- [Testing](#-testing)
- [Contribuir](#-contribuir)
- [Licencia](#-licencia)

---

## 🚀 Características

### Implementado ✅
- [x] Autenticación JWT
- [x] Gestión de Usuarios
- [x] Publicaciones (Posts)
- [x] Manejo centralizado de errores
- [x] Logging con Serilog + MongoDB
- [x] Cache en memoria
- [x] Almacenamiento de imágenes con Cloudinary
- [x] Inyección de dependencias

### En desarrollo 🔄
- [ ] Comentarios y respuestas
- [ ] Sistema de likes y retweets
- [ ] Búsqueda y filtros
- [ ] Notificaciones
- [ ] Mensajería directa
- [ ] Hashtags y tendencias

### Planificado 📋
- [ ] Unit Tests
- [ ] Integration Tests
- [ ] API Documentation (Swagger)
- [ ] Rate Limiting
- [ ] Health Checks
- [ ] WebSocket para notificaciones en tiempo real

---

## 🛠️ Stack Tecnológico

### Backend
- **Framework:** .NET 8 / C# 12
- **ORM:** Entity Framework Core
- **Base de Datos Principal:** SQL Server
- **Base de Datos de Logs:** MongoDB
- **Autenticación:** JWT Bearer Tokens
- **Package Manager:** NuGet

### Servicios Externos
- **Almacenamiento de Imágenes:** Cloudinary
- **Logging:** Serilog

### Herramientas de Desarrollo
- **IDE:** Visual Studio Code / Visual Studio 2022
- **Versionado:** Git

---

## 📁 Estructura del Proyecto

```
XClone-Backend/
├── X.Domain/                          # Capa de Dominio
│   ├── Entities/                      # Modelos de negocio
│   │   └── User.cs
│   ├── Enums/                         # Enumeraciones
│   │   └── UserStatusEnum.cs
│   ├── Exceptions/                    # Excepciones personalizadas
│   │   ├── BadRequestException.cs
│   │   └── NotFoundException.cs
│   ├── Interfaces/Repository/         # Contratos del repositorio
│   └── Value Objects/                 # Value Objects del dominio
│
├── X.Application/                     # Capa de Aplicación
│   ├── Modules/                       # Casos de uso organizados por feature
│   │   ├── Auth/
│   │   │   ├── LogIn/
│   │   │   └── Logout/
│   │   ├── User/
│   │   │   └── CreateUser/
│   │   └── Post/
│   ├── Interfaces/                    # Contratos/interfaces
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Validators/                    # Validadores de comandos
│   └── DependencyInjection.cs         # Configuración de servicios
│
├── X.Infrastructure/                  # Capa de Infraestructura
│   ├── Persistence/                   # Modelos de base de datos
│   │   ├── User.cs
│   │   ├── Post.cs
│   │   └── ...
│   ├── Repository/                    # Implementación del patrón Repository
│   │   └── UserRepository.cs
│   ├── Services/                      # Servicios externos
│   │   ├── Jwt.cs
│   │   ├── Password.cs
│   │   ├── CacheService.cs
│   │   └── ImageStorage.cs
│   ├── Database/SqlServer/            # Contexto de Entity Framework
│   ├── env/                           # Variables de configuración
│   │   ├── ConfigurationConstants.cs
│   │   └── TokenConfiguration.cs
│   └── DependencyInjection.cs         # Configuración de servicios
│
├── X.WebApi/                          # Capa de Presentación
│   ├── Controllers/                   # Endpoints REST
│   │   ├── AuthController.cs
│   │   └── UserController.cs
│   ├── DTOs/Request/                  # DTOs de entrada
│   ├── Middlewares/                   # Middlewares personalizados
│   │   └── ErrorHandlerMiddleware.cs
│   ├── Program.cs                     # Configuración principal
│   ├── appsettings.json               # Configuración
│   ├── appsettings.Development.json   # Configuración de desarrollo
│   └── DependencyInjection.cs         # Configuración de servicios
│
├── X.Shared/                          # Código compartido
│   ├── Constants/                     # Constantes globales
│   │   ├── ResponseConstant.cs
│   │   └── ValidationConstants.cs
│   ├── Helpers/                       # Funciones auxiliares
│   │   └── ResponseHelper.cs
│   ├── Responses/                     # Modelos de respuesta
│   │   └── GenericResponse.cs
│   └── DependencyInjection.cs
│
├── .env                               # Variables de entorno (NO versionar)
├── X.slnx                             # Solution file
├── README.md                          # Este archivo
├── EVALUACION_TECNICA_SENIOR.md       # Evaluación del proyecto
├── ANALISIS_ARQUITECTURA.md           # Análisis arquitectónico detallado
├── GUIA_CONFIGURACION_JWT.md          # Guía de configuración JWT
└── GUIA_IMPLEMENTACION_CONSTANTES.md  # Guía de constantes
```

---

## ⚡ Quick Start

### Prerrequisitos

- **.NET 8 SDK** ([Descargar](https://dotnet.microsoft.com/download))
- **SQL Server 2019+** o **Docker**
- **Node.js 18+** (para herramientas auxiliares)
- **Git**

### 1. Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/xclone-backend.git
cd xclone-backend
```

### 2. Configurar Variables de Entorno

Copia el archivo `.env.example` a `.env`:

```bash
cp .env.example .env
```

Edita `.env` con tus valores:

```env
# Base de Datos
DefaultConnection=Server=localhost;Database=XCloneDb;Trusted_Connection=true;

# JWT
JWT:Key=tu-clave-secreta-muy-larga-minimo-32-caracteres
JWT:Issuer=xclone-backend
JWT:Audience=xclone-client
JWT:ExpireMinutes=60

# MongoDB (para logs)
MongoDb:ConnectionString=mongodb://localhost:27017

# Cloudinary
Cloudinary:CloudName=tu-cloud-name
Cloudinary:ApiKey=tu-api-key
Cloudinary:ApiSecret=tu-api-secret
```

### 3. Restaurar Dependencias

```bash
dotnet restore
```

### 4. Crear Migración de Base de Datos

```bash
# Crear la base de datos
dotnet ef database update
```

### 5. Ejecutar la Aplicación

```bash
cd X.WebApi
dotnet run
```

La API estará disponible en: `https://localhost:5001`

---

## ⚙️ Configuración

### Estructura de Configuración

La configuración se maneja en tres niveles:

#### 1. **appsettings.json** (General)
```json
{
  "JWT": {
    "Key": "tu-clave-secreta",
    "Issuer": "xclone-backend",
    "Audience": "xclone-client",
    "ExpireMinutes": 60
  }
}
```

#### 2. **.env** (Variables Sensibles - NO versionar)
```env
DefaultConnection=...
JWT:Key=...
```

#### 3. **ConfigurationConstants.cs** (Constantes)
```csharp
public static class ConfigurationConstants
{
    public const string JwtKey = "JWT:Key";
    public const string JwtExpireMinutes = "JWT:ExpireMinutes";
    // ...
}
```

### Variables de Entorno Requeridas

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `DefaultConnection` | Connection string SQL Server | `Server=localhost;...` |
| `JWT:Key` | Clave secreta JWT (mín. 32 chars) | `my-secret-key-...` |
| `JWT:Issuer` | Emisor del token | `xclone-backend` |
| `JWT:Audience` | Audiencia autorizada | `xclone-client` |
| `JWT:ExpireMinutes` | Expiración del token | `60` |
| `MongoDb:ConnectionString` | URI de MongoDB | `mongodb://localhost` |
| `Cloudinary:CloudName` | Cloud name | `tu-cloud` |
| `Cloudinary:ApiKey` | API Key Cloudinary | `tu-api-key` |
| `Cloudinary:ApiSecret` | API Secret Cloudinary | `tu-secret` |

---

## 📡 API Endpoints

### Autenticación

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}

Response:
{
  "data": {
    "accessToken": "eyJ0eXAiOiJKV1QiLCJhbGc...",
    "refreshToken": "eyJ0eXAiOiJKV1QiLCJhbGc..."
  },
  "message": "Inicio de sesión exitoso",
  "timeStamp": "2026-04-09T14:30:00Z",
  "errors": []
}
```

#### Logout
```http
POST /api/auth/logout
Authorization: Bearer <token>

Response:
{
  "message": "Sesión cerrada exitosamente",
  "timeStamp": "2026-04-09T14:30:00Z"
}
```

### Usuarios

#### Crear Usuario
```http
POST /api/users/register
Content-Type: application/json

{
  "email": "newuser@example.com",
  "username": "newuser",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePassword123!"
}

Response:
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "newuser@example.com",
    "username": "newuser"
  },
  "message": "Usuario creado exitosamente",
  "timeStamp": "2026-04-09T14:30:00Z"
}
```

### Publicaciones

#### Crear Post
```http
POST /api/posts
Authorization: Bearer <token>
Content-Type: application/json

{
  "content": "¡Hola mundo en XClone!",
  "imageUrls": []
}

Response:
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "content": "¡Hola mundo en XClone!",
    "createdAt": "2026-04-09T14:30:00Z"
  },
  "message": "Publicación creada exitosamente",
  "timeStamp": "2026-04-09T14:30:00Z"
}
```

---

## 🧪 Testing

### Ejecutar Tests

```bash
# Unit Tests
dotnet test XClone.Tests.Unit

# Integration Tests
dotnet test XClone.Tests.Integration

# Todos los tests
dotnet test
```

### Cobertura de Tests

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

---

## 📚 Documentación Adicional

- **[EVALUACION_TECNICA_SENIOR.md](./EVALUACION_TECNICA_SENIOR.md)** - Evaluación técnica y plan de mejoras
- **[ANALISIS_ARQUITECTURA.md](./ANALISIS_ARQUITECTURA.md)** - Análisis detallado de la arquitectura
- **[GUIA_CONFIGURACION_JWT.md](./GUIA_CONFIGURACION_JWT.md)** - Configuración segura de JWT
- **[GUIA_IMPLEMENTACION_CONSTANTES.md](./GUIA_IMPLEMENTACION_CONSTANTES.md)** - Uso de constantes

---

## 🤝 Contribuir

### Verificación Previa al Commit

Antes de hacer commit, asegúrate de:

```bash
# 1. Formatear código
dotnet format

# 2. Ejecutar tests
dotnet test

# 3. Verificar que no hay errores
dotnet build
```

### Ramas de Desarrollo

- `main` - Producción
- `develop` - Desarrollo
- `feature/*` - Nuevas características
- `bugfix/*` - Correcciones de bugs
- `docs/*` - Documentación

### Proceso de Contribución

1. Fork el repositorio
2. Crea una rama: `git checkout -b feature/nueva-caracteristica`
3. Haz commits descriptivos: `git commit -m "feat: agregar nueva caracteristica"`
4. Push a la rama: `git push origin feature/nueva-caracteristica`
5. Abre un Pull Request

---

## 📋 Issues Conocidos / CRÍTICOS

> ⚠️ Lee **EVALUACION_TECNICA_SENIOR.md** para una lista completa de issues

### 🔴 Bloqueantes
- [ ] JWT con expiration aleatorio (CRÍTICO - SEGURIDAD)
- [ ] JWT configurado en capa incorrecta
- [ ] Magic strings en handlers

### 🟠 Importantes
- [ ] Sin tests unitarios
- [ ] Sin validación de entrada con FluentValidation
- [ ] Logging descentralizado

---

## 📞 Soporte

- **Issues:** [GitHub Issues](https://github.com/tu-usuario/xclone-backend/issues)
- **Email:** developer@xclone.com
- **Documentación:** [Wiki](https://wiki.xclone.com)

---

## 📄 Licencia

Este proyecto está bajo licencia MIT. Ver [LICENSE](./LICENSE) para más detalles.

---

## 🙏 Agradecimientos

- Clean Architecture por Uncle Bob
- .NET Community
- Todos los contribuidores

---

**Última actualización:** 9 de abril de 2026  
**Versión:** 1.0.0  
**Mantenedor:** Senior Developer Team

---

### Checklist para Nuevos Desarrolladores

- [ ] Cloné el repositorio
- [ ] Configuré el .env
- [ ] Corrí `dotnet restore`
- [ ] Configuré la BD
- [ ] Ejecuté `dotnet run`
- [ ] Leí EVALUACION_TECNICA_SENIOR.md
- [ ] Leí ANALISIS_ARQUITECTURA.md

¡Bienvenido al equipo! 🚀
