# ✅ Checklist de Calidad - XClone Backend

**Propósito:** Verificar que el código cumple con estándares de calidad  
**Frecuencia:** Antes de cada Pull Request  
**Versión:** 1.0

---

## 🏗️ Checklist de Arquitectura

```
[ ] Código en la capa correcta
    [ ] Lógica de negocio → X.Domain o X.Application
    [ ] Servicios externos → X.Infrastructure
    [ ] Endpoints → X.WebApi
    [ ] Constantes/Helpers → X.Shared

[ ] Dependencias correctas
    [ ] X.WebApi NO depende de Infrastructure directamente
    [ ] X.Domain NO tiene dependencias externas
    [ ] X.Application NO referencia Controllers
    [ ] X.Infrastructure NO referencia X.WebApi

[ ] Patrones utilizados correctamente
    [ ] Repository pattern para acceso a datos
    [ ] Inyección de dependencias para servicios
    [ ] Handlers para casos de uso
    [ ] Excepciones personalizadas para errores
    [ ] Constantes centralizadas
```

---

## 🔒 Checklist de Seguridad

```
[ ] Secrets seguros
    [ ] No hay secrets en .cs files
    [ ] No hay secrets en appsettings.json (solo en .env)
    [ ] .env está en .gitignore
    [ ] Contraseñas hasheadas en BD

[ ] Validación de entrada
    [ ] Todos los DTOs tienen validación
    [ ] Validación de longitud máxima
    [ ] Validación de formato (email, etc)
    [ ] Input sanitizado

[ ] Autenticación/Autorización
    [ ] JWT con expiration FIJO (no aleatorio)
    [ ] JWT con tamaño mínimo de 32 caracteres
    [ ] Tokens en Authorization header
    [ ] Refresh tokens implementados (si aplica)
    [ ] HTTPS forzado en producción

[ ] Manejo de datos sensibles
    [ ] No loguear passwords, tokens, SSN
    [ ] No exponer detalles internos en errores
    [ ] Rate limiting en endpoints públicos
```

---

## 🧹 Checklist de Clean Code

```
[ ] Nomenclatura
    [ ] Clases en PascalCase (UserController)
    [ ] Métodos en PascalCase (GetUser)
    [ ] Variables en camelCase (userName)
    [ ] Constantes en UPPER_SNAKE_CASE (USER_NOT_FOUND)
    [ ] Nombres descriptivos (no x, y, temp)

[ ] Métodos
    [ ] Un solo criterio de responsabilidad
    [ ] <20 líneas de código
    [ ] Máximo 3 parámetros
    [ ] Sin lógica duplicada

[ ] Código DRY (Don't Repeat Yourself)
    [ ] Sin mensajes de error duplicados
    [ ] Sin lógica duplicada
    [ ] Usar constantes para valores repetidos
    [ ] Extraer métodos reutilizables

[ ] Complejidad
    [ ] Máximo 3 niveles de anidamiento
    [ ] Sin métodos muy largos
    [ ] Sin condicionales muy complejos
    [ ] Usar early returns

[ ] Comentarios
    [ ] Comentarios explican el POR QUÉ, no el QUÉ
    [ ] Documentación XML en métodos públicos
    [ ] README del proyecto actualizado
```

---

## 📝 Checklist de Documentación

```
[ ] Métodos públicos
    [ ] Tengan documentación XML
    [ ] Describan parámetros y retorno
    [ ] Mencionen excepciones que lanzan
    [ ] Tengan ejemplos si es complejo

[ ] Archivos propios de negocio
    [ ] Tengan comentario de descripción
    [ ] Expliquen responsabilidades
    [ ] Citen referencias de diseño

[ ] Documentación general
    [ ] README.md actualizado
    [ ] GUIA_SETUP.md funcional
    [ ] Guía de contribución existe
    [ ] Problemas conocidos documentados

[ ] Commits
    [ ] Mensaje descriptivo en español
    [ ] Referencias a issues (si aplica)
    [ ] Commits atómicos (un cambio por commit)
```

---

## 🧪 Checklist de Testing

```
[ ] Unit Tests
    [ ] Cada handler tiene tests
    [ ] Cada servicio tiene tests
    [ ] Cada validator tiene tests
    [ ] Cobertura >80%

[ ] Integration Tests
    [ ] Repositorios testeados
    [ ] Endpoints testeados
    [ ] Flujos de negocio testeados

[ ] Test Quality
    [ ] Nombres descriptivos
    [ ] AAA pattern (Arrange, Act, Assert)
    [ ] Fixtures reutilizables
    [ ] Datos de prueba claros
    [ ] Sin dependencias entre tests

[ ] Antes de commit
    [ ] `dotnet test` pasa sin errores
    [ ] Cobertura no baja
    [ ] Todos los tests ejecutan bien
```

---

## 🎯 Checklist pre-PR (Pull Request)

```
[ ] Código
    [ ] Compila sin errores
    [ ] Sin warnings
    [ ] Formateado correctamente (`dotnet format`)
    [ ] Sin archivos .sln o .csproj innecesarios

[ ] Tests
    [ ] Tests pasan
    [ ] Cobertura > 80%
    [ ] Casos edge testeados

[ ] Documentación
    [ ] README actualizado
    [ ] Comentarios en código complejo
    [ ] Cambios arquitectónicos documentados

[ ] Git
    [ ] Branch limpio (sin commits basura)
    [ ] Commits tienen mensajes claros
    [ ] .env NO está commiteado
    [ ] Binarios NO están commiteados

[ ] Seguridad
    [ ] Sin secrets expuestos
    [ ] Sin passwords en code
    [ ] Sin commented code innecesario
    [ ] Validación de input presente

[ ] Descripción del PR
    [ ] Describe qué cambió
    [ ] Explica por qué cambió
    [ ] Menciona issues relacionados
    [ ] Menciona cambios breaking
```

---

## 🔧 Automatización

### pre-commit Hook (opcional)

Crear archivo `.git/hooks/pre-commit`:

```bash
#!/bin/bash

# Formatar código
dotnet format

# Ejecutar tests
dotnet test

# Si tests fallan, no permitir commit
if [ $? -ne 0 ]; then
    echo "❌ Tests fallaron. Commit cancelado."
    exit 1
fi

echo "✅ Pre-commit checks pasaron"
exit 0
```

Hacer ejecutable:
```bash
chmod +x .git/hooks/pre-commit
```

---

## 📊 Matriz de Decisión

| Aspecto | Se Rechaza | Se Acepta |
|---------|-----------|----------|
| **Tests** | <70% cobertura | >80% cobertura |
| **Mensajes** | Magic strings | Constantes |
| **Métodos** | >30 líneas | <20 líneas |
| **Documentación** | Sin XML docs | Con XML docs |
| **Secrets** | En código | En .env |
| **Errores** | Swallowed | Loguean/Lanzan |

---

## 🚀 Flujo de Revisión

```
1. Developer hace cambios
   ↓
2. Ejecuta checklist (local)
   ↓
3. Hace commit + Push
   ↓
4. CI/CD verifica (automático)
   - Compila
   - Tests
   - Linting
   ↓
5. Peer Review
   - Usa este checklist
   - Verifica arquitectura
   - Verifica seguridad
   ↓
6. Merge si todo OK
```

---

## 📞 Referencias

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Clean Code - Uncle Bob](https://www.oreilly.com/library/view/clean-code-a/9780136083238/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Última actualización:** 9 de abril de 2026

