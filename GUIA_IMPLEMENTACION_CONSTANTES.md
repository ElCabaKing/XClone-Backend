# 📋 Guía de Implementación Rápida: Constantes y Helpers

## 🎯 Objetivo
Centralizar strings, mensajes y configuración para mantener el código DRY (Don't Repeat Yourself).

---

## 1️⃣ Paso 1: Ampliar `ResponseConstants.cs`

**Archivo:** `X.Shared/Constants/ResponseConstants.cs`

```csharp
namespace X.Shared.Constants;

/// <summary>
/// Constantes centralizadas para mensajes de respuesta
/// Usar en TODOS los controladores, handlers y servicios
/// </summary>
public static class ResponseConstants
{
    // ========== MENSAJES DE ÉXITO ==========
    public const string SUCCESS = "Solicitud realizada correctamente";
    public const string USER_CREATED_SUCCESSFULLY = "Usuario creado exitosamente";
    public const string LOGIN_SUCCESSFUL = "Inicio de sesión exitoso";
    public const string LOGOUT_SUCCESSFUL = "Sesión cerrada exitosamente";
    public const string PASSWORD_CHANGED_SUCCESSFULLY = "Contraseña cambiada exitosamente";
    public const string PROFILE_UPDATED_SUCCESSFULLY = "Perfil actualizado exitosamente";
    public const string EMAIL_VERIFIED_SUCCESSFULLY = "Email verificado exitosamente";
    public const string POST_CREATED_SUCCESSFULLY = "Publicación creada exitosamente";
    
    // ========== ERRORES DE AUTENTICACIÓN ==========
    public const string LOGIN_ERROR = "Correo o contraseña incorrectos";
    public const string USER_NOT_FOUND = "Usuario no encontrado";
    public const string USER_ALREADY_EXISTS = "El usuario ya existe";
    public const string INVALID_EMAIL = "El correo electrónico no es válido";
    public const string INVALID_PASSWORD = "La contraseña no cumple los requisitos de seguridad";
    public const string INVALID_CREDENTIALS = "Las credenciales son inválidas";
    public const string ACCOUNT_DISABLED = "La cuenta ha sido desactivada";
    public const string EMAIL_NOT_VERIFIED = "El email no ha sido verificado";
    public const string TOKEN_EXPIRED = "El token ha expirado";
    public const string INVALID_TOKEN = "El token es inválido";
    
    // ========== ERRORES DE AUTORIZACIÓN ==========
    public const string UNAUTHORIZED = "No está autorizado para realizar esta acción";
    public const string FORBIDDEN = "Acceso prohibido";
    public const string INSUFFICIENT_PERMISSIONS = "Permisos insuficientes";
    
    // ========== ERRORES DE VALIDACIÓN ==========
    public const string INVALID_INPUT = "Los datos ingresados no son válidos";
    public const string MISSING_REQUIRED_FIELD = "Campo requerido: {0}";
    public const string INVALID_FORMAT = "Formato inválido: {0}";
    public const string LENGTH_OUT_OF_RANGE = "La longitud debe estar entre {0} y {1}";
    
    // ========== ERRORES DE NEGOCIO ==========
    public const string COLLABORATOR_NOT_EXISTS = "El colaborador no existe";
    public const string PROJECT_NOT_EXISTS = "El proyecto no existe";
    public const string POST_NOT_FOUND = "Publicación no encontrada";
    public const string COMMENT_NOT_FOUND = "Comentario no encontrado";
    public const string LIKE_ALREADY_EXISTS = "Ya has likeado esta publicación";
    public const string FOLLOW_ALREADY_EXISTS = "Ya estás siguiendo a este usuario";
    
    // ========== ERRORES DE SISTEMA ==========
    public static string ERROR_UNEXPECTED(string traceId) =>
        $"Ha ocurrido un error inesperado. Código de error: {traceId}. Contacte con soporte.";
    
    public const string DATABASE_ERROR = "Error en la base de datos";
    public const string SERVICE_UNAVAILABLE = "El servicio no está disponible";
    public const string FILE_UPLOAD_ERROR = "Error al subir el archivo";
    public const string INVALID_FILE_TYPE = "Tipo de archivo no permitido";
}
```

---

## 2️⃣ Paso 2: Crear Helpers de Configuración

**Archivo:** `X.Infrastructure/Configuration/ConfigurationHelper.cs`

```csharp
using Microsoft.Extensions.Configuration;

namespace X.Infrastructure.Configuration;

/// <summary>
/// Extensiones para IConfiguration para simplificar acceso a valores
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// Obtiene un valor de configuración requerido y lanza excepción si no existe
    /// </summary>
    /// <example>
    /// var jwtKey = config.GetRequiredValue("JWT:Key");
    /// </example>
    public static string GetRequiredValue(
        this IConfiguration config,
        string key)
    {
        return config[key]
            ?? throw new InvalidOperationException(
                $"Configuration key '{key}' is not configured.");
    }

    /// <summary>
    /// Obtiene un valor de configuración con sección y key separados
    /// </summary>
    /// <example>
    /// var cloudName = config.GetRequiredValue("CloudName", "Cloudinary");
    /// </example>
    public static string GetRequiredValue(
        this IConfiguration config,
        string section,
        string key)
    {
        var value = config[$"{section}:{key}"];
        return value
            ?? throw new InvalidOperationException(
                $"Configuration key '{section}:{key}' is not configured.");
    }

    /// <summary>
    /// Obtiene un valor con valor por defecto
    /// </summary>
    /// <example>
    /// var timeout = config.GetValueOrDefault("TimeoutSeconds", "30");
    /// </example>
    public static string GetValueOrDefault(
        this IConfiguration config,
        string key,
        string defaultValue)
    {
        return config[key] ?? defaultValue;
    }

    /// <summary>
    /// Obtiene un entero de configuración
    /// </summary>
    /// <example>
    /// var pageSize = config.GetInt("PageSize", 10);
    /// </example>
    public static int GetInt(
        this IConfiguration config,
        string key,
        int defaultValue = 0)
    {
        var value = config[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Obtiene un booleano de configuración
    /// </summary>
    /// <example>
    /// var isProduction = config.GetBool("IsProduction", false);
    /// </example>
    public static bool GetBool(
        this IConfiguration config,
        string key,
        bool defaultValue = false)
    {
        var value = config[key];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Obtiene un TimeSpan de configuración
    /// </summary>
    /// <example>
    /// var timeout = config.GetTimeSpan("TimeoutMinutes", TimeSpan.FromMinutes(5));
    /// </example>
    public static TimeSpan GetTimeSpan(
        this IConfiguration config,
        string key,
        TimeSpan defaultValue)
    {
        var value = config[key];
        return TimeSpan.TryParse(value, out var result) ? result : defaultValue;
    }
}
```

**Archivo:** `X.Infrastructure/Configuration/ConfigurationValidator.cs`

```csharp
using Microsoft.Extensions.Configuration;

namespace X.Infrastructure.Configuration;

/// <summary>
/// Valida que todas las configuraciones requeridas existan en startup
/// </summary>
public static class ConfigurationValidator
{
    /// <summary>
    /// Valida la configuración en tiempo de startup
    /// Lanza InvalidOperationException si faltan claves requeridas
    /// </summary>
    /// <example>
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.Configuration.ValidateRequiredConfigurations();
    /// </example>
    public static void ValidateRequiredConfigurations(this IConfiguration configuration)
    {
        var requiredKeys = new[]
        {
            // JWT
            "JWT:Key",
            "JWT:Issuer",
            "JWT:Audience",
            "JWT:ExpireMinutes",
            
            // Database
            "DefaultConnection",
            "MongoDb:ConnectionString",
            
            // Cloudinary
            "Cloudinary:CloudName",
            "Cloudinary:ApiKey",
            "Cloudinary:ApiSecret",
        };

        var missingKeys = requiredKeys
            .Where(key => string.IsNullOrEmpty(configuration[key]))
            .ToList();

        if (missingKeys.Count > 0)
        {
            var missingKeysList = string.Join(", ", missingKeys);
            throw new InvalidOperationException(
                $"Configuration keys are missing: {missingKeysList}");
        }
    }
}
```

---

## 3️⃣ Paso 3: Actualizar `ResponseHelper.cs`

**Archivo:** `X.Shared/Helpers/ResponseHelper.cs`

```csharp
using X.Shared.Constants;
using X.Shared.Responses;

namespace X.Shared.Helpers;

/// <summary>
/// Helper para crear respuestas genéricas
/// Siempre usar este helper en lugar de instanciar GenericResponse directamente
/// </summary>
public static class ResponseHelper
{
    /// <summary>
    /// Crea una respuesta exitosa con datos
    /// </summary>
    /// <example>
    /// return ResponseHelper.Create(user, message: ResponseConstants.USER_CREATED_SUCCESSFULLY);
    /// </example>
    public static GenericResponse<T> Create<T>(
        T data,
        string? message = null,
        List<string>? errors = null)
    {
        return new GenericResponse<T>
        {
            Data = data,
            Message = message ?? ResponseConstants.SUCCESS,
            Errors = errors ?? []
        };
    }

    /// <summary>
    /// Crea una respuesta de error
    /// </summary>
    /// <example>
    /// return ResponseHelper.Error<User>(ResponseConstants.USER_NOT_FOUND);
    /// </example>
    public static GenericResponse<T> Error<T>(
        string errorMessage,
        T? data = default)
    {
        return new GenericResponse<T>
        {
            Data = data!,
            Message = errorMessage,
            Errors = [errorMessage]
        };
    }

    /// <summary>
    /// Crea una respuesta con múltiples errores
    /// </summary>
    /// <example>
    /// return ResponseHelper.Errors<User>(
    ///     "Validation failed",
    ///     new List<string> { "Email is required", "Password is weak" }
    /// );
    /// </example>
    public static GenericResponse<T> Errors<T>(
        string message,
        List<string> errors,
        T? data = default)
    {
        return new GenericResponse<T>
        {
            Data = data!,
            Message = message,
            Errors = errors
        };
    }
}
```

---

## 4️⃣ Paso 4: Reemplazar Magic Strings en Handlers

### ❌ ANTES

**Archivo:** `X.Application/Modules/Auth/LogIn/UserLogInHandler.cs`

```csharp
public class UserLogInHandler(IToken tokenService, 
    IPasswordHash passwordHash,
    IUserRepository userRepository)
{
    public async Task<GenericResponse<string>> Execute(UserLogInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            throw new BadRequestException("User or password is incorrect");  // ❌ Magic string
        }

        if (!passwordHash.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new BadRequestException("User or password is incorrect");  // ❌ Magic string
        }
        return ResponseHelper.Create(tokenService.GenerateToken(user.Id.ToString()));  // ❌ Sin mensaje específico
    }
}
```

### ✅ DESPUÉS

```csharp
using X.Shared.Constants;
using X.Shared.Helpers;

namespace X.Application.Modules.Auth.LogIn;

public class UserLogInHandler(
    IToken tokenService,
    IPasswordHash passwordHash,
    IUserRepository userRepository)
{
    public async Task<GenericResponse<string>> Execute(UserLogInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            throw new BadRequestException(ResponseConstants.LOGIN_ERROR);  // ✅ Constante
        }

        if (!passwordHash.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new BadRequestException(ResponseConstants.LOGIN_ERROR);  // ✅ Constante
        }

        var token = tokenService.GenerateToken(user.Id.ToString());
        
        return ResponseHelper.Create(
            token,
            message: ResponseConstants.LOGIN_SUCCESSFUL  // ✅ Constante
        );
    }
}
```

---

## 5️⃣ Paso 5: Actualizar `Program.cs`

**Archivo:** `X.WebApi/Program.cs`

```csharp
using X.Application;
using X.Infrastructure;
using X.Infrastructure.Configuration;  // ✅ Nuevo usando
using X.WebApi.Middlewares;
using Serilog;
using X.WebApi;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;

Env.Load(".env");
Env.Load("../X.Infrastructure/.env");

var builder = WebApplication.CreateBuilder(args);

// ✅ Validar configuración requerida en startup
builder.Configuration.ValidateRequiredConfigurations();

builder.Host.UseSerilog();
builder.Services.AddOpenApi();

// Usar nuevo helper para registrar servicios (antes y después en DependencyInjection.cs)
builder.Services
    .AddWebApi(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
```

---

## 6️⃣ Paso 6: Actualizar `DependencyInjection.cs` en Infrastructure

**Archivo:** `X.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using X.Application.Interfaces;
using X.Domain.Interfaces.Repository;
using X.Infrastructure.Repository;
using X.Infrastructure.Services;
using X.Infrastructure.Configuration;  // ✅ Nuevo usando
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using X.Infrastructure.env;

namespace X.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddCache(services);
        ConfigureCloudinary(services, configuration);
        ConfigureRepositories(services);
        ConfigureServices(services);

        return services;
    }

    private static void ConfigureCloudinary(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ Usar ConfigurationHelper para código limpio
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
        services.AddScoped<IToken, Jwt>();
    }

    public static void AddCache(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
    }
}
```

---

## 📋 Checklist de Implementación

- [ ] Ampliar `ResponseConstants.cs` con todos los mensajes
- [ ] Crear `ConfigurationHelper.cs`
- [ ] Crear `ConfigurationValidator.cs`
- [ ] Actualizar `ResponseHelper.cs`
- [ ] Reemplazar magic strings en `UserLogInHandler.cs`
- [ ] Reemplazar magic strings en `CreateUserHandler.cs`
- [ ] Actualizar `Program.cs` con validación
- [ ] Actualizar `DependencyInjection.cs` para usar helpers
- [ ] Buscar y reemplazar otros magic strings en repositorios y servicios
- [ ] Actualizar tests para usar constantes
- [ ] Documentar patrón en CONTRIBUTING.md

---

## 🚀 Beneficios Inmediatos

✅ **Código más limpio y mantenible**
✅ **Sin duplicación de mensajes de error**
✅ **Fácil localización (i18n) en el futuro**
✅ **Menos bugs por typos**
✅ **Búsqueda rápida de donde se usan mensajes**
✅ **Validación de configuración en startup**

---

## 📚 Recursos

- [Microsoft - Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration)
- [C# - Static Classes and Static Class Members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members)
- [Internationalization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)
