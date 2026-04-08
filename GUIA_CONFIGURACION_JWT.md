# 🔐 Análisis y Corrección: Mala Configuración de JWT

**Estado Actual:** ❌ MAL CONFIGURADO  
**Severidad:** 🔴 CRÍTICA  
**Impacto:** Seguridad, Mantenibilidad, Testing

---

## 🎯 El Problema

### 1. **JWT configurado en la capa INCORRECTA** ❌

**Ubicación Actual (INCORRECTA):**
```
X.WebApi/
└── DependencyInjection.cs  ❌ Configuración de autenticación aquí
```

**Problema:**
- JWT es un **servicio de infraestructura** (librería externa), NO de API
- La configuración de autenticación debe estar en `X.Infrastructure`
- Viola el principio de Clean Architecture
- La capa WebApi NO debe conocer detalles de autenticación

**Impacto:**
- ❌ Difícil de reusabilizar en otros proyectos (CLI, Workers, etc.)
- ❌ Acoplamiento entre WebApi e Infrastructure
- ❌ Difícil de testear
- ❌ Configuración esparcida en múltiples lugares

---

### 2. **TokenConfiguration mal diseñada** ❌

**Código Actual (INCORRECTO):**
```csharp
public class TokenConfiguration
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public required string TokenKey { get; set; }  // ❌ Nunca se usa
    public required DateTime Expiration { get; set; } = DateTime.UtcNow.AddMinutes(
        Random.Shared.Next(1, 5)  // ❌ Aleatorio! Incorrecto
    );
}
```

**Problemas:**
- ❌ `TokenKey` está declarado pero nunca se usa
- ❌ `Expiration` es un `DateTime` aleatorio (¡SEGURIDAD!)
- ❌ Debería ser `ExpirationMinutes` (int)
- ❌ Los valores por defecto no tienen sentido
- ❌ No hay validación

---

### 3. **Interfaz IToken limitada** ❌

**Código Actual (INCOMPLETO):**
```csharp
public interface IToken
{
    string GenerateToken(string userId);  // ❌ Solo userId
}
```

**Problemas:**
- ❌ No permite agregar claims personalizados
- ❌ No permite expiration custom
- ❌ No permite refresh tokens
- ❌ No hay método para validar/decodificar tokens

---

## ✅ Solución Propuesta

### Arquitectura Correcta

```
X.Infrastructure/
├── DependencyInjection.cs  ✅ Configuración de JWT aquí
├── Configuration/
│   ├── JwtConfiguration.cs  ✅ Clase de configuración mejorada
│   └── JwtConfigurationValidator.cs  ✅ Validación
├── Authentication/
│   ├── ITokenService.cs  ✅ Interfaz mejorada
│   ├── JwtTokenService.cs  ✅ Implementación
│   └── TokenGeneratorHelper.cs  ✅ Helper para generar tokens fácilmente
└── Services/
    └── [Otros servicios]

X.WebApi/
├── DependencyInjection.cs  ✅ Solo registra servicios de API
└── Program.cs  ✅ Solo llama a métodos de setup
```

---

## 🛠️ Implementación Step by Step

### Paso 1: Crear `JwtConfiguration` mejorada

**Archivo:** `X.Infrastructure/Configuration/JwtConfiguration.cs`

```csharp
namespace X.Infrastructure.Configuration;

/// <summary>
/// Configuración segura para JWT
/// Se carga desde appsettings.json
/// </summary>
public class JwtConfiguration
{
    /// <summary>
    /// Clave secreta para firmar tokens (mínimo 32 caracteres recomendado)
    /// </summary>
    public required string SecretKey { get; set; }

    /// <summary>
    /// Identificador del emisor del token
    /// </summary>
    public required string Issuer { get; set; }

    /// <summary>
    /// Identificador autorizado para usar el token
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// Minutos que durará el token (recomendado: 15-60 minutos)
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Minutos que durará el refresh token (recomendado: 7 días = 10080 minutos)
    /// </summary>
    public int RefreshTokenExpirationMinutes { get; set; } = 10080;

    /// <summary>
    /// Validar que la configuración sea correcta
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new InvalidOperationException("JWT SecretKey is required");

        if (SecretKey.Length < 32)
            throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT Issuer is required");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT Audience is required");

        if (ExpirationMinutes <= 0)
            throw new InvalidOperationException("JWT ExpirationMinutes must be greater than 0");

        if (RefreshTokenExpirationMinutes <= ExpirationMinutes)
            throw new InvalidOperationException(
                "RefreshTokenExpirationMinutes must be greater than ExpirationMinutes");
    }
}
```

---

### Paso 2: Mejorar la interfaz `IToken`

**Archivo:** `X.Application/Interfaces/IToken.cs`

```csharp
using System.Security.Claims;

namespace X.Application.Interfaces;

/// <summary>
/// Servicio para generar y validar tokens JWT
/// Debe implementarse en Infrastructure
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token JWT con el userId
    /// </summary>
    string GenerateToken(string userId);

    /// <summary>
    /// Genera un token JWT con claims personalizados
    /// </summary>
    string GenerateToken(string userId, Dictionary<string, object>? claims = null);

    /// <summary>
    /// Genera un token JWT con expiration personalizada
    /// </summary>
    string GenerateToken(
        string userId,
        TimeSpan? expiration = null,
        Dictionary<string, object>? claims = null);

    /// <summary>
    /// Genera un refresh token
    /// </summary>
    string GenerateRefreshToken(string userId);

    /// <summary>
    /// Valida un token y retorna los claims
    /// </summary>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Decodifica un token y retorna los claims sin validar firma
    /// </summary>
    Dictionary<string, string>? DecodeToken(string token);

    /// <summary>
    /// Obtiene el userId de un token
    /// </summary>
    string? GetUserIdFromToken(string token);
}
```

---

### Paso 3: Implementar `JwtTokenService`

**Archivo:** `X.Infrastructure/Services/JwtTokenService.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using X.Application.Interfaces;
using X.Infrastructure.Configuration;

namespace X.Infrastructure.Services;

/// <summary>
/// Servicio para generar y validar JWT tokens
/// Reemplaza la clase Jwt anterior
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly JwtConfiguration _config;

    public JwtTokenService(JwtConfiguration config)
    {
        _config = config;
        _config.Validate();  // Validar en tiempo de construcción
    }

    /// <summary>
    /// Genera un token JWT básico con solo el userId
    /// </summary>
    public string GenerateToken(string userId)
    {
        return GenerateToken(userId, null, null);
    }

    /// <summary>
    /// Genera un token JWT con claims personalizados
    /// </summary>
    public string GenerateToken(string userId, Dictionary<string, object>? claims = null)
    {
        return GenerateToken(userId, null, claims);
    }

    /// <summary>
    /// Genera un token JWT con todas las opciones
    /// </summary>
    public string GenerateToken(
        string userId,
        TimeSpan? expiration = null,
        Dictionary<string, object>? claims = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config.SecretKey);

        var claimsIdentity = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("sub", userId)
        };

        // Agregar claims personalizados
        if (claims != null)
        {
            foreach (var claim in claims)
            {
                claimsIdentity.Add(new Claim(claim.Key, claim.Value.ToString() ?? string.Empty));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimsIdentity),
            Expires = DateTime.UtcNow.AddMinutes(
                expiration?.TotalMinutes ?? _config.ExpirationMinutes),
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Genera un refresh token
    /// </summary>
    public string GenerateRefreshToken(string userId)
    {
        var expirationMinutes = _config.RefreshTokenExpirationMinutes;
        return GenerateToken(
            userId,
            TimeSpan.FromMinutes(expirationMinutes),
            new Dictionary<string, object> { { "type", "refresh" } });
    }

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config.SecretKey);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config.Issuer,
                ValidateAudience = true,
                ValidAudience = _config.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Decodifica un token sin validar la firma (solo para lectura)
    /// ADVERTENCIA: No usar para validación de seguridad
    /// </summary>
    public Dictionary<string, string>? DecodeToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.Claims
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => x.First().Value);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extrae el userId de un token
    /// </summary>
    public string? GetUserIdFromToken(string token)
    {
        var claims = DecodeToken(token);
        if (claims == null)
            return null;

        return claims.TryGetValue(ClaimTypes.NameIdentifier, out var userId)
            ? userId
            : claims.TryGetValue("sub", out var subject)
                ? subject
                : null;
    }
}
```

---

### Paso 4: Crear `TokenGeneratorHelper`

**Archivo:** `X.Infrastructure/Services/TokenGeneratorHelper.cs`

```csharp
using X.Application.Interfaces;

namespace X.Infrastructure.Services;

/// <summary>
/// Helper para simplificar la generación de tokens con opciones comunes
/// Proporciona métodos de conveniencia para casos de uso típicos
/// </summary>
public static class TokenGeneratorHelper
{
    /// <summary>
    /// Genera un token de acceso con claims de usuario
    /// </summary>
    /// <example>
    /// var token = TokenGeneratorHelper.GenerateAccessToken(
    ///     _tokenService,
    ///     userId: "123",
    ///     email: "user@example.com",
    ///     roles: new[] { "admin", "user" }
    /// );
    /// </example>
    public static string GenerateAccessToken(
        ITokenService tokenService,
        string userId,
        string? email = null,
        string[]? roles = null,
        Dictionary<string, object>? additionalClaims = null)
    {
        var claims = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(email))
            claims.Add("email", email);

        if (roles != null && roles.Length > 0)
            claims.Add("roles", string.Join(",", roles));

        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
                claims.Add(claim.Key, claim.Value);
        }

        return tokenService.GenerateToken(userId, claims);
    }

    /// <summary>
    /// Genera un token con expiration corta (para operaciones sensibles)
    /// </summary>
    public static string GenerateShortLivedToken(
        ITokenService tokenService,
        string userId,
        int expirationMinutes = 5)
    {
        return tokenService.GenerateToken(
            userId,
            TimeSpan.FromMinutes(expirationMinutes),
            new Dictionary<string, object> { { "type", "short-lived" } });
    }

    /// <summary>
    /// Genera un token de confirmación de email
    /// </summary>
    public static string GenerateEmailConfirmationToken(
        ITokenService tokenService,
        string userId,
        string email)
    {
        return tokenService.GenerateToken(
            userId,
            TimeSpan.FromHours(24),
            new Dictionary<string, object>
            {
                { "type", "email-confirmation" },
                { "email", email }
            });
    }

    /// <summary>
    /// Genera un token de reset de contraseña
    /// </summary>
    public static string GeneratePasswordResetToken(
        ITokenService tokenService,
        string userId)
    {
        return tokenService.GenerateToken(
            userId,
            TimeSpan.FromHours(1),
            new Dictionary<string, object> { { "type", "password-reset" } });
    }
}
```

---

### Paso 5: Actualizar `Infrastructure/DependencyInjection.cs`

**Archivo:** `X.Infrastructure/DependencyInjection.cs`

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Repository;
using X.Infrastructure.Services;
using X.Infrastructure.Configuration;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;

namespace X.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ Configurar JWT primero (necesario antes de autenticación)
        ConfigureJwt(services, configuration);

        AddCache(services);
        ConfigureCloudinary(services, configuration);
        ConfigureRepositories(services);
        ConfigureServices(services);

        return services;
    }

    /// <summary>
    /// Configura JWT en la capa correcta (Infrastructure)
    /// </summary>
    private static void ConfigureJwt(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ Cargar configuración
        var jwtConfig = new JwtConfiguration();
        configuration.GetSection("JWT").Bind(jwtConfig);
        jwtConfig.Validate();  // Validar en startup

        // ✅ Registrar configuración
        services.AddSingleton(jwtConfig);
        services.AddScoped<ITokenService, JwtTokenService>();

        // ✅ Configurar autenticación
        var key = Encoding.UTF8.GetBytes(jwtConfig.SecretKey);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // ✅ Configurar autorización
        services.AddAuthorization();
    }

    private static void ConfigureCloudinary(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var cloudName = configuration.GetRequiredValue(
            ConfigurationConstants.CloudinaryCloudName);
        var apiKey = configuration.GetRequiredValue(
            ConfigurationConstants.CloudinaryApiKey);
        var apiSecret = configuration.GetRequiredValue(
            ConfigurationConstants.CloudinaryApiSecret);

        var account = new Account(cloudName, apiKey, apiSecret);
        var cloudinary = new Cloudinary(account);

        services.AddSingleton(cloudinary);
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IPasswordHash, Password>();
        services.AddScoped<IStorage, ImageStorage>();
    }

    public static void AddCache(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
    }
}
```

---

### Paso 6: Simplificar `WebApi/DependencyInjection.cs`

**Archivo:** `X.WebApi/DependencyInjection.cs`

```csharp
using X.WebApi.Middlewares;
using Serilog;
using X.Infrastructure.Database.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using X.Infrastructure.env;

namespace X.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddScoped<ErrorHandlerMiddleware>();

        // ✅ El logging se sigue haciendo aquí (es específico de WebApi)
        ConfigureLogger(services, configuration);

        // ✅ La base de datos se sigue haciendo aquí (aunque podría estar en Infrastructure)
        ConfigureDB(services, configuration);

        return services;
    }

    private static void ConfigureLogger(
        IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.MongoDB(
                configuration[ConfigurationConstants.MongoConnectionString] ??
                throw new InvalidOperationException("MongoDB Connection String is not configured."),
                collectionName: "Logs")
            .CreateLogger();
    }

    private static void ConfigureDB(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<XDbContext>(options =>
            options.UseSqlServer(
                configuration[ConfigurationConstants.ConnectionString] ??
                throw new InvalidOperationException("SQL Server Connection String is not configured.")));
    }
}
```

---

### Paso 7: Actualizar `Program.cs`

**Archivo:** `X.WebApi/Program.cs`

```csharp
using X.Application;
using X.Infrastructure;
using X.WebApi.Middlewares;
using Serilog;
using X.WebApi;
using DotNetEnv;
using System.Globalization;

Env.Load(".env");
Env.Load("../X.Infrastructure/.env");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// ✅ Registrar servicios en el orden correcto
// 1. Infrastructure (con JWT)
// 2. Application (use cases)
// 3. WebApi (controllers y middlewares)
builder.Services
    .AddInfrastructure(builder.Configuration)  // JWT configurado aquí
    .AddApplication()
    .AddWebApi(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ✅ Use middleware en el orden correcto
app.UseMiddleware<ErrorHandlerMiddleware>();

// ✅ Usar autenticación (ahora viene de Infrastructure)
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
```

---

### Paso 8: Actualizar Handlers para usar el nuevo servicio

**Archivo:** `X.Application/Modules/Auth/LogIn/UserLogInHandler.cs`

```csharp
using X.Application.Interfaces;
using X.Domain.Exceptions;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Services;
using X.Shared.Constants;
using X.Shared.Helpers;
using X.Shared.Responses;

namespace X.Application.Modules.Auth.LogIn;

public class UserLogInHandler(
    ITokenService tokenService,  // ✅ Cambió de IToken a ITokenService
    IPasswordHash passwordHash,
    IUserRepository userRepository)
{
    public async Task<GenericResponse<string>> Execute(UserLogInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            throw new BadRequestException(ResponseConstants.LOGIN_ERROR);
        }

        if (!passwordHash.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new BadRequestException(ResponseConstants.LOGIN_ERROR);
        }

        // ✅ Usar helper para generar token con claims
        var token = TokenGeneratorHelper.GenerateAccessToken(
            tokenService,
            userId: user.Id.ToString(),
            email: user.Email,
            roles: new[] { "user" }
        );

        return ResponseHelper.Create(
            token,
            message: ResponseConstants.LOGIN_SUCCESSFUL
        );
    }
}
```

---

### Paso 9: Actualizar `appsettings.json`

**Archivo:** `X.WebApi/appsettings.json`

```json
{
  "JWT": {
    "SecretKey": "your-super-secret-key-at-least-32-characters-long-change-this!!!!",
    "Issuer": "XClone",
    "Audience": "XCloneUsers",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationMinutes": 10080
  },
  "DefaultConnection": "Server=localhost;Database=XClone;Trusted_Connection=true;",
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 📊 Comparativa: Antes vs Después

| Aspecto | ❌ ANTES | ✅ DESPUÉS |
|---------|---------|----------|
| **JWT configurado en** | WebApi (Incorrecto) | Infrastructure (Correcto) |
| **Interfaz IToken** | Limitada (solo GenerateToken) | Completa (7 métodos) |
| **TokenConfiguration** | Defectuosa | Validada y robusta |
| **Claims personalizados** | No soportados | Completamente soportados |
| **Refresh tokens** | No existe | Método dedicado |
| **Validación de token** | No existe | ValidateToken + DecodeToken |
| **Helper para tokens** | No existe | TokenGeneratorHelper completo |
| **Seguridad** | Baja (Expiration aleatoria) | Alta (validación en construcción) |
| **Testing** | Difícil | Fácil (desacoplado) |

---

## ✨ Beneficios de Aplicar

✅ **Arquitectura correcta** - JWT en Infrastructure  
✅ **Seguridad mejorada** - Configuración validada en startup  
✅ **Funcionalidad extendida** - Claims, refresh tokens, validación  
✅ **Code clarity** - Helpers para casos comunes  
✅ **Fácil testing** - Servicios desacoplados  
✅ **Reusable** - Puede usarse en CLI, Workers, etc.  
✅ **Mantenibilidad** - Una sola fuente de verdad  

---

## 🚀 Orden de Implementación

1. [ ] Crear `JwtConfiguration.cs`
2. [ ] Crear `JwtTokenService.cs`
3. [ ] Crear `TokenGeneratorHelper.cs`
4. [ ] Actualizar `ITokenService` (interfaz)
5. [ ] Actualizar `Infrastructure/DependencyInjection.cs`
6. [ ] Simplificar `WebApi/DependencyInjection.cs`
7. [ ] Actualizar `Program.cs`
8. [ ] Actualizar handlers para usar `ITokenService`
9. [ ] Actualizar `appsettings.json`
10. [ ] Eliminar clase `Jwt.cs` antigua
11. [ ] Eliminar clase `TokenConfiguration.cs` antigua
12. [ ] Tests

---

## 📋 Checklist de Validación

- [ ] JWT configurado en Infrastructure, no en WebApi
- [ ] `JwtConfiguration` valida en startup
- [ ] `ITokenService` soporta todos los métodos propuestos
- [ ] `TokenGeneratorHelper` simplifica casos comunes
- [ ] `Program.cs` llama a `app.UseAuthentication()`
- [ ] `app.UseAuthorization()` está después de `app.UseAuthentication()`
- [ ] Claims se incluyen correctamente en los tokens
- [ ] Refresh tokens funcionan (expiration > access token)
- [ ] `appsettings.json` tiene SecretKey de 32+ caracteres
- [ ] Tests para `JwtTokenService` creados
- [ ] Controllers protegidos con `[Authorize]`

---

## 🔒 Recomendaciones de Seguridad

**(APLICAR EN PRODUCCIÓN)**

```json
{
  "JWT": {
    "SecretKey": "USAR_ENVIRONMENT_VARIABLE_EN_PRODUCCION",
    "Issuer": "XClone",
    "Audience": "XCloneUsers",
    "ExpirationMinutes": 15,
    "RefreshTokenExpirationMinutes": 10080
  }
}
```

```csharp
// En Program.cs
var secretKey = configuration["JWT:SecretKey"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT secret key must be configured");
```

---

## 📚 Recursos

- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Microsoft JWT Bearer Documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens)
- [OWASP - JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)
