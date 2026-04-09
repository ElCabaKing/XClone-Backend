# 🧪 Guía de Testing - XClone Backend

**Versión:** 1.0  
**Estado:** En desarrollo - Tests aún no implementados  
**Prioridad:** ALTA (Roadmap Fase 2)

---

## 📋 Tabla de Contenidos

1. [Visión General](#-visión-general)
2. [Estructura de Tests](#-estructura-de-tests)
3. [Setup de Testing](#-setup-de-testing)
4. [Unit Tests](#-unit-tests)
5. [Integration Tests](#-integration-tests)
6. [Mejores Prácticas](#-mejores-prácticas)
7. [Cobertura de Tests](#-cobertura-de-tests)

---

## 👁️ Visión General

### Estado Actual ⚠️
- ❌ **0% Cobertura** - Sin tests implementados
- ❌ **Sin proyecto de tests** - No existe carpeta de tests

### Estado Deseado ✅
- ✅ **>80% Cobertura** - Meta mínima
- ✅ **Unit Tests** - Para lógica de negocio
- ✅ **Integration Tests** - Para repositorios y servicios
- ✅ **API Tests** - Para endpoints

### Beneficios
- ✅ Confianza en cambios sin romper funcionalidad
- ✅ Documentación viva del código
- ✅ Detección temprana de bugs
- ✅ Refactorización segura

---

## 📁 Estructura de Tests

```
XClone-Backend/
├── XClone.Tests/
│   ├── XClone.Tests.Unit/           # Unit Tests
│   │   ├── XClone.Tests.Unit.csproj
│   │   ├── Fixtures/                # Datos de prueba
│   │   ├── Modules/
│   │   │   ├── Auth/
│   │   │   │   └── UserLogInHandlerTests.cs
│   │   │   ├── User/
│   │   │   │   └── CreateUserHandlerTests.cs
│   │   │   └── Post/
│   │   ├── Services/
│   │   │   ├── JwtServiceTests.cs
│   │   │   ├── PasswordServiceTests.cs
│   │   │   └── CacheServiceTests.cs
│   │   └── Helpers/
│   │       └── ResponseHelperTests.cs
│   │
│   └── XClone.Tests.Integration/    # Integration Tests
│       ├── XClone.Tests.Integration.csproj
│       ├── Fixtures/                # Test fixtures y database
│       ├── Repositories/
│       │   ├── UserRepositoryTests.cs
│       │   └── PostRepositoryTests.cs
│       ├── API/
│       │   ├── AuthEndpointTests.cs
│       │   └── UserEndpointTests.cs
│       └── Services/
│           └── UserServiceTests.cs
│
└── X.slnx (actualizar para incluir proyectos de test)
```

---

## 🔧 Setup de Testing

### Paso 1: Crear Proyectos de Test

```bash
# Desde la raíz del proyecto
mkdir -p XClone.Tests

# Crear proyecto de Unit Tests
dotnet new xunit -n XClone.Tests.Unit -o XClone.Tests/XClone.Tests.Unit

# Crear proyecto de Integration Tests
dotnet new xunit -n XClone.Tests.Integration -o XClone.Tests/XClone.Tests.Integration

# Agregar los proyectos a la solución
dotnet sln add XClone.Tests/XClone.Tests.Unit/XClone.Tests.Unit.csproj
dotnet sln add XClone.Tests/XClone.Tests.Integration/XClone.Tests.Integration.csproj
```

### Paso 2: Instalar Paquetes de Testing

```bash
cd XClone.Tests/XClone.Tests.Unit

# xUnit - Framework principal
dotnet add package xunit --version 2.4.2

# Moq - Mock objects
dotnet add package Moq --version 4.18.4

# FluentAssertions - Assertions más legibles
dotnet add package FluentAssertions --version 7.0.0

# Bogus - Fake data generators
dotnet add package Bogus --version 35.0.0

# AutoFixture - Test data builders
dotnet add package AutoFixture --version 4.18.0
```

```bash
cd ../XClone.Tests.Integration

# xUnit
dotnet add package xunit --version 2.4.2

# Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0

# TestContainers (para SQL Server en tests)
dotnet add package Testcontainers --version 3.0.0
dotnet add package Testcontainers.MsSql --version 3.0.0

# FluentAssertions
dotnet add package FluentAssertions --version 7.0.0

# Referencias
dotnet add reference ../../X.Infrastructure/X.Infrastructure.csproj
dotnet add reference ../../X.Application/X.Application.csproj
```

---

## ✏️ Unit Tests

### Estructura Básica

```csharp
using Xunit;
using FluentAssertions;
using Moq;
using XClone.Application.Interfaces;
using XClone.Domain.Exceptions;
using XClone.Application.Modules.Auth.LogIn;

namespace XClone.Tests.Unit.Modules.Auth;

/// <summary>
/// Tests para UserLogInHandler
/// Naming: [ClassUnderTest]Tests
/// </summary>
public class UserLogInHandlerTests
{
    // Arrange: Setup de datos
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHash> _passwordHashMock;
    private readonly Mock<IToken> _tokenServiceMock;
    private readonly UserLogInHandler _handler;

    public UserLogInHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHashMock = new Mock<IPasswordHash>();
        _tokenServiceMock = new Mock<IToken>();
        
        _handler = new UserLogInHandler(
            _tokenServiceMock.Object,
            _passwordHashMock.Object,
            _userRepositoryMock.Object
        );
    }

    // Naming: [MethodName]_[Scenario]_[ExpectedResult]
    [Fact]
    public async Task Execute_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var userEmail = "test@example.com";
        var password = "SecurePassword123!";
        var userId = Guid.NewGuid();
        
        var user = new User(
            id: userId,
            username: "testuser",
            email: userEmail,
            passwordHash: \"hashed_password\",
            createdAt: DateTime.UtcNow,
            isVerified: true,
            statusId: UserStatusEnum.Active,
            firstName: "Test\",
            lastName: \"User\",
            profilePictureUrl: null
        );

        var command = new UserLogInCommand(userEmail, password);
        var expectedToken = \"valid.jwt.token\";

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(userEmail))
            .ReturnsAsync(user);

        _passwordHashMock
            .Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(true);

        _tokenServiceMock
            .Setup(x => x.GenerateToken(userId.ToString()))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Execute(command);

        // Assert
        result.Should().NotBeNull();
        result.Data.AccessToken.Should().Be(expectedToken);
        _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(userEmail), Times.Once);
        _passwordHashMock.Verify(x => x.VerifyPassword(password, user.PasswordHash), Times.Once);
    }

    [Fact]
    public async Task Execute_WithInvalidEmail_ThrowsBadRequestException()
    {
        // Arrange
        var command = new UserLogInCommand(\"invalid@example.com\", \"password\");
        
        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Execute(command));
    }

    [Fact]
    public async Task Execute_WithInvalidPassword_ThrowsBadRequestException()
    {
        // Arrange
        var user = new User(...);  // Crear usuario
        var command = new UserLogInCommand(user.Email, \"wrongpassword\");

        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _passwordHashMock
            .Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Execute(command));
    }
}
```

### Mejores Prácticas en Unit Tests

1. **Naming:** `[MethodUnderTest]_[Scenario]_[ExpectedResult]`
   ```csharp
   // ✅ BIEN
   public void GenerateToken_WithValidUserId_ReturnsNonEmptyString()
   
   // ❌ MAL
   public void TestGenerateToken()
   ```

2. **AAA Pattern** - Arrange, Act, Assert
   ```csharp
   // Arrange - Preparar datos
   var user = new User(...);
   
   // Act - Ejecutar la acción
   var result = service.GenerateToken(user.Id);
   
   // Assert - Verificar el resultado
   result.Should().NotBeNullOrEmpty();
   ```

3. **Mocking de Dependencias**
   ```csharp
   var mockRepository = new Mock<IUserRepository>();
   mockRepository
       .Setup(x => x.GetUserByEmailAsync(email))
       .ReturnsAsync(user);
   ```

4. **Assertions Claros**
   ```csharp
   // ✅ BIEN - FluentAssertions
   result.Should().NotBeNull().And.BeOfType<UserLogInResponse>();
   result.AccessToken.Should().NotBeNullOrEmpty();
   
   // ❌ DIFÍCIL DE LEER
   Assert.NotNull(result);
   Assert.True(!string.IsNullOrEmpty(result.AccessToken));
   ```

---

## 🔌 Integration Tests

### Ejemplo: UserRepository Tests

```csharp
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using XClone.Infrastructure.Database.SqlServer.Context;
using XClone.Infrastructure.Persistence;
using XClone.Infrastructure.Repository;
using XClone.Domain.Enums;

namespace XClone.Tests.Integration.Repositories;

public class UserRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly XDbContext _dbContext;
    private readonly UserRepository _repository;

    public UserRepositoryIntegrationTests()
    {
        // Usar InMemory database para tests
        var options = new DbContextOptionsBuilder<XDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new XDbContext(options);
        _repository = new UserRepository(_dbContext);
    }

    public async Task InitializeAsync()
    {
        // Preparar datos antes de cada test
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Limpiar después de cada test
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User(
            id: Guid.NewGuid(),
            username: \"testuser\",
            email: \"test@example.com\",
            passwordHash: \"hashed\",
            createdAt: DateTime.UtcNow,
            isVerified: true,
            statusId: UserStatusEnum.Active,
            firstName: \"Test\",
            lastName: \"User\",
            profilePictureUrl: null
        );

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByEmailAsync(\"test@example.com\");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(\"test@example.com\");
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Act
        var result = await _repository.GetUserByEmailAsync(\"nonexistent@example.com\");

        // Assert
        result.Should().BeNull();
    }
}
```

---

## 🧪 Mejores Prácticas

### 1. Test Naming Convention
```
[UnitOfWork]_[Scenario]_[ExpectedBehavior]

Ejemplos:
✅ PasswordService_WithValidPassword_ReturnsTrue
✅ UserRepository_WithNonExistentId_ReturnsNull
❌ TestPassword
❌ Test1
```

### 2. Principios FIRST
- **F**ast - Ejecutar rápido (< milisegundos)
- **I**ndependent - Sin dependencias entre tests
- **R**epeatable - Resultado consistente
- **S**elf-checking - Pasa/Falla automáticamente
- **T**imely - Escritos antes o con el código

### 3. Test Data Builders (Fluent API)

```csharp
public class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _email = \"test@example.com\";
    private string _username = \"testuser\";
    private bool _isVerified = true;

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithVerified(bool isVerified)
    {
        _isVerified = isVerified;
        return this;
    }

    public User Build()
    {
        return new User(
            _id, _username, _email, \"hashed\", DateTime.UtcNow,
            _isVerified, UserStatusEnum.Active, \"First\", \"Last\", null
        );
    }
}

// Uso en tests
[Fact]
public void Test()
{
    var user = new UserBuilder()
        .WithEmail(\"custom@example.com\")
        .WithVerified(false)
        .Build();

    // Usar `user`...
}
```

### 4. Fixtures para Setup Común

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private XDbContext _dbContext;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<XDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new XDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    public XDbContext GetContext() => _dbContext;
}

// Uso
public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
}
```

---

## 📊 Cobertura de Tests

### Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Tests específicos
dotnet test --filter ClassName=UserLogInHandlerTests

# Con output detallado
dotnet test -v n

# Con coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Generar Reporte de Coverage

```bash
# Instalar herramienta global
dotnet tool install -g dotnet-reportgenerator-globaltool

# Correr tests con coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Generar reporte HTML
reportgenerator -reports:\"**/coverage.opencover.xml\" \
  -targetdir:CoverageReport -reporttypes:Html
```

### Target de Cobertura

| Componente | Target |
|-----------|--------|
| **Handlers** | 85%+ |
| **Services** | 80%+ |
| **Repositories** | 80%+ |
| **Validators** | 90%+ |
| **Utilities** | 70%+ |
| **TOTAL** | 80%+ |

---

## 🚀 Fases de Implementación

### Fase 1: Fundamentos (1 semana)
- [ ] Crear estructura de proyectos de test
- [ ] Instalar dependencias
- [ ] Crear fixtures básicas
- [ ] Tests para Auth module (LoginHandler)

### Fase 2: Cobertura Principal (2 semanas)
- [ ] Tests para User module
- [ ] Tests para Post module
- [ ] Tests para Repositories
- [ ] Tests para Services

### Fase 3: Integration & E2E (1 semana)
- [ ] Integration tests
- [ ] API endpoint tests
- [ ] Performance tests

---

## 📞 Recursos

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq GitHub](https://github.com/moq/moq4)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)

---

**Próxima actualización:** Cuando se implementen los primeros tests

