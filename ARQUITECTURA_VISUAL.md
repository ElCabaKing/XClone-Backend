# 📐 Arquitectura Visual - XClone Backend

**Versión:** 1.0  
**Propósito:** Visualizar relaciones y flujos entre capas  
**Audiencia:** Nuevos desarrolladores, Architects

---

## 🏛️ Capas y Dependencias

```
┌─────────────────────────────────────────────────────────────┐
│                    X.WebApi (Presentation)                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Controllers  │ DTOs  │ Middlewares  │ DependencyInjection│
│  │ - AuthController    │ - ErrorHandlerMiddleware     │   │
│  │ - UserController    │ - CustomMiddleware           │   │
│  │ - PostController    │                              │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↓
                     (Referencia HTTP)
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                 X.Application (Use Cases)                   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Commands   │ Handlers  │ Interfaces  │ Validators   │   │
│  │ - UserLogInCommand                                  │   │
│  │ - CreateUserCommand                                 │   │
│  │ - UserLogInHandler                                  │   │
│  │ - CreateUserHandler                                 │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                    ↓            ↓
         (IUserRepository)  (IToken, IPasswordHash)
                    ↓            ↓
┌──────────────────────────────────────────────────────────────┐
│                   X.Infrastructure                           │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ Repository    │ Services      │ Persistence        │    │
│  │ - UserRepository  │ - JwtTokenService  │ - User DB │   │
│  │ - PostRepository  │ - PasswordService  │ - Post DB │   │
│  │                   │ - CacheService    │ - Comment │   │
│  │                   │ - ImageStorage    │ - Like DB │   │
│  └──────────────────────────────────────────────────────┘    │
│  Database: SQL Server                                        │
│  MongoDB Logs                                                │
│  Cloudinary Storage                                          │
└──────────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────────┐
│                      X.Domain (Entities)                     │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ Entities       │ Interfaces    │ Exceptions        │    │
│  │ - User.cs      │ - IRepository │ - BadRequest...   │   │
│  │ - Post.cs      │ - IService    │ - NotFound...     │   │
│  │ - Comment.cs   │ - IToken      │                   │   │
│  │                │               │                   │   │
│  │ Enums          │ Value Objects │                   │   │
│  │ - UserStatusEnum               │                   │   │
│  └──────────────────────────────────────────────────────┘    │
│  ⚠️ NO tiene dependencias externas                           │
└──────────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────────┐
│                     X.Shared (Utilidades)                    │
│  ┌──────────────────────────────────────────────────────┐    │
│  │ Constants  │ Helpers  │ Responses  │ Validators    │    │
│  │ - ResponseConstant │ - ResponseHelper             │    │
│  │ - ValidationConstants                              │   │
│  │ - GenericResponse<T>                                │   │
│  └──────────────────────────────────────────────────────┘    │
│  Usado por TODAS las capas                                   │
└──────────────────────────────────────────────────────────────┘
```

---

## 🔄 Flujo de Autenticación (Login)

```
┌─────────────────────────────────────────────────────────────┐
│ Cliente                                                     │
│ POST /api/auth/login                                        │
│ { email: "user@example.com", password: "pass123" }         │
└─────────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────────┐
│ AuthController.LogIn()                                      │
│ - Recibe UserLogInRequestDTO                               │
│ - Crea UserLogInCommand                                    │
└─────────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────────┐
│ UserLogInHandler.Execute()                                 │
│ - Inyección: IToken, IPasswordHash, IUserRepository       │
└─────────────────────────────────────────────────────────────┘
                         ↓
              ┌──────────┴──────────┐
              ↓                     ↓
    ┌──────────────────┐  ┌──────────────────┐
    │ IUserRepository  │  │ IPasswordHash    │
    │ .GetByEmailAsync │  │ .VerifyPassword  │
    └──────────────────┘  └──────────────────┘
              ↓                     ↓
    ┌──────────────────┐  ┌──────────────────┐
    │ UserRepository   │  │ Password Service │
    │ (SQL Server)     │  │ (bcrypt)         │
    └──────────────────┘  └──────────────────┘
              ↓                     ↓
         ✅ User found        ✅ Password OK
              └──────────────┬──────────────┘
                             ↓
    ┌──────────────────────────────────────────┐
    │ IToken.GenerateToken(userId.ToString()) │
    │ - Crea JWT con claims                   │
    │ - Firma con secret key                  │
    └──────────────────────────────────────────┘
                             ↓
    ┌──────────────────────────────────────────┐
    │ Retorna UserLogInResponse                │
    │ {                                        │
    │   accessToken: "eyJ0eXAiOi...",         │
    │   refreshToken: "eyJ0eXAiOi..."         │
    │ }                                        │
    └──────────────────────────────────────────┘
                             ↓
    ┌──────────────────────────────────────────┐
    │ ResponseHelper.Create(response)          │
    │ - Envuelve en GenericResponse<T>        │
    │ - Agrega timestamp y mensaje            │
    └──────────────────────────────────────────┘
                             ↓
    ┌──────────────────────────────────────────┐
    │ Client recibe:                           │
    │ {                                        │
    │   data: { accessToken, refreshToken },  │
    │   message: "Inicio de sesión exitoso",  │
    │   timeStamp: "2026-04-09T14:30:00Z",    │
    │   errors: []                             │
    │ }                                        │
    └──────────────────────────────────────────┘
```

---

## 🚨 Manejo de Errores

```
┌─────────────────────────────────────────────────────────┐
│ Excepción lanzada en Handler                           │
│ throw new BadRequestException("Email o password...")   │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│ ErrorHandlerMiddleware captura la excepción           │
│ - Determina el tipo                                   │
│ - Genera error response                               │
│ - Genera TraceId                                      │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│ Retorna al cliente:                                    │
│ {                                                      │
│   data: null,                                          │
│   message: "",                                         │
│   timeStamp: "2026-04-09T14:30:00Z",                 │
│   errors: ["Email o password incorrecto"]            │
│ }                                                      │
│ Status: 400 BadRequest                                │
└─────────────────────────────────────────────────────────┘
```

---

## 📦 Inyección de Dependencias

```
┌─────────────────────────────────────────────────────────┐
│ Program.cs                                             │
│ - AddApplication()                                    │
│ - AddInfrastructure()                                 │
│ - AddWebApi()                                         │
└─────────────────────────────────────────────────────────┘
                         ↓
    ┌────────────────┬──────────────────┬───────────────┐
    ↓                ↓                  ↓               ↓
┌──────────┐   ┌──────────────┐   ┌───────────┐   ┌─────────┐
│Application│   │Infrastructure│   │  WebApi  │   │ Shared  │
│ DI        │   │ DI           │   │  DI      │   │ (global)│
└──────────┘   └──────────────┘   └───────────┘   └─────────┘
    │               │
    │               ├─ AddScoped<IUserRepository, UserRepository>
    │               ├─ AddScoped<IToken, Jwt>
    │               ├─ AddScoped<IPasswordHash, Password>
    │               ├─ AddScoped<IStorage, ImageStorage>
    │               └─ AddScoped<ICacheService, CacheService>
    │
    ├─ AddScoped<UserLogInHandler>
    ├─ AddScoped<CreateUserHandler>
    └─ ... (más handlers)
```

---

## 🗄️ Modelo de Datos

```
┌──────────────────────────────┐
│          Users               │
├──────────────────────────────┤
│ Id (GUID)              [PK]  │
│ Username               [UQ]  │
│ Email                  [UQ]  │
│ FirstName                    │
│ LastName                     │
│ PasswordHash                 │
│ CreatedAt                    │
│ UpdatedAt                    │
│ IsVerified (bool)            │
│ ProfilePictureUrl            │
│ StatusId (FK) ────────┐      │
└──────────────────────┼──────┘
                       │
                    ┌──▼──────────────────────────────────┐
                    │       UserStatus                     │
                    ├──────────────────────────────────────┤
                    │ Id (int)              [PK]           │
                    │ Status (Active|Banned|Suspended)    │
                    └───────────────────────────────────────┘

┌──────────────────────────────┐
│          Posts               │
├──────────────────────────────┤
│ Id (GUID)              [PK]  │
│ UserId (FK) ────────────┐    │
│ Content (string)         │    │
│ CreatedAt                │    │
│ UpdatedAt                │    │
│ LikeCount                │    │
│ CommentCount             │    │
└────────────────┼─────────────┘
                 │
                 └─→ Users (Relación)

┌──────────────────────────────┐
│       Comments               │
├──────────────────────────────┤
│ Id (GUID)              [PK]  │
│ PostId (FK)                  │
│ UserId (FK)                  │
│ Content (string)             │
│ CreatedAt                    │
└──────────────────────────────┘

┌──────────────────────────────┐
│        Likes                 │
├──────────────────────────────┤
│ Id (GUID)              [PK]  │
│ PostId (FK)                  │
│ UserId (FK)                  │
│ CreatedAt                    │
│ UNIQUE(PostId, UserId)   [UQ]│
└──────────────────────────────┘
```

---

## 🔐 Flujo de Seguridad (JWT)

```
LOGIN
├─ Cliente: POST /login
├─ Server: Verifica email/password
└─ Server: Genera JWT
   │
   ├─ Payload: { sub: userId, claims: [...] }
   ├─ Firma: HMACSHA256(header + payload, secret_key)
   └─ Retorna: eyJ0eXAiOi...

AUTHENTICATED REQUEST
├─ Cliente: GET /api/protected
├─ Header: Authorization: Bearer <token>
├─ Server: Extrae token
├─ Server: Valida firma
├─ Server: Verifica expiration
├─ Server: Extrae claims
└─ Server: Procesa solicitud

TOKEN EXPIRADO
├─ Cliente recibe: 401 Unauthorized
├─ Cliente usa: RefreshToken
├─ Server: Valida RefreshToken
└─ Server: Genera nuevo AccessToken
```

---

## 📊 Configuración por Ambiente

```
DESARROLLO
├─ DefaultConnection → localhost:1433
├─ JWT:Key → (clave de desarrollo)
├─ JWT:ExpireMinutes → 60
├─ MongoDB → localhost:27017
├─ Logging → Console + MongoDB
└─ CORS → localhost:3000, localhost:5173

STAGING
├─ DefaultConnection → staging-server
├─ JWT:Key → (clave de staging)
├─ JWT:ExpireMinutes → 30
├─ MongoDB → staging-mongo
├─ Logging → MongoDB
└─ CORS → staging.app.com

PRODUCCIÓN
├─ DefaultConnection → prod-server (encrypted)
├─ JWT:Key → (clave muy segura)
├─ JWT:ExpireMinutes → 15
├─ MongoDB → prod-mongo (secured)
├─ Logging → MongoDB + Azure AppInsights
├─ CORS → *.ejemplo.com
└─ HTTPS → Forzado
```

---

## 🔄 Ciclo de Vida del Request

```
1. HTTP Request llega
   ↓
2. Middleware chain (FIFO)
   ├─ Authentication Middleware
   ├─ Authorization Middleware
   ├─ CORS Middleware
   ├─ ErrorHandler Middleware
   └─ ...
   ↓
3. Routing → Encuentra Controller
   ↓
4. Model Binding → Construye DTO
   ↓
5. Validación (Data Annotations)
   ↓
6. Controller Action ejecuta
   ├─ Crea Command
   ├─ Llama Handler
   ├─ Handler inyecta dependencias
   ├─ Handler ejecuta lógica
   └─ Retorna resultado
   ↓
7. Response Helper envuelve resultado
   ↓
8. Serialización JSON
   ↓
9. Status Code + Headers
   ↓
10. HTTP Response retorna al cliente
```

---

## 🎯 Responsabilidad por Capa

```
X.Domain (Reglas de Negocio)
├─ ✅ Lógica pura de negocio
├─ ✅ Entidades y Value Objects
├─ ✅ Excepciones personalizadas
├─ ✅ Interfaces (contratos)
└─ ❌ NO acceso a datos
   ❌ NO servicios externos

X.Application (Casos de Uso)
├─ ✅ Orquestación de lógica
├─ ✅ Validación de comandos
├─ ✅ Transformación de datos (DTOs)
├─ ✅ Llamadas a repositorios
└─ ❌ NO HTTP
   ❌ NO base de datos directo

X.Infrastructure (Detalles Técnicos)
├─ ✅ Acceso a datos (Repository)
├─ ✅ Servicios externos
├─ ✅ Configuración de BD
├─ ✅ Implementación de interfaces
└─ ❌ NO lógica de negocio
   ❌ NO HTTP

X.WebApi (Presentación)
├─ ✅ Endpoints HTTP
├─ ✅ Middlewares
├─ ✅ Manejo inicial de requests
├─ ✅ DTOs de entrada/salida
└─ ❌ NO lógica de negocio
   ❌ NO acceso a datos directo

X.Shared (Utilidades)
├─ ✅ Constantes globales
├─ ✅ Helpers reutilizables
├─ ✅ Respuestas genéricas
└─ ❌ NO lógica específica del negocio
```

---

## 📚 Referencias

- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Relationships](https://docs.microsoft.com/en-us/ef/core/modeling/relationships)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)

---

**Última actualización:** 9 de abril de 2026
