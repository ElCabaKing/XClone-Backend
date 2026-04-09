# 🔧 Guía de Setup - XClone Backend

**Versión:** 1.0  
**Última actualización:** 9 de abril de 2026  
**Audiencia:** Nuevos desarrolladores

---

## 📋 Tabla de Contenidos

1. [Instalación de Herramientas](#-instalación-de-herramientas)
2. [Configuración Inicial](#-configuración-inicial)
3. [Ejecutar en Desarrollo](#-ejecutar-en-desarrollo)
4. [Troubleshooting](#-troubleshooting)
5. [Verificación de Setup](#-verificación-de-setup)

---

## 🛠️ Instalación de Herramientas

### Requisitos Mínimos

| Herramienta | Versión | Verificar |
|-------------|---------|----------|
| .NET SDK | 8.0+ | `dotnet --version` |
| SQL Server | 2019+ | SQL Server Manager |
| Git | 2.30+ | `git --version` |
| Node.js | 18+ | `node --version` |

### 1. Instalar .NET 8 SDK

#### En Windows
```bash
# Opción 1: Instalador
# Descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0
# Ejecutar el instalador

# Opción 2: Chocolatey (si lo tienes)
choco install dotnet-sdk-8.0
```

#### En macOS
```bash
# Con Homebrew
brew install dotnet-sdk@8

# O descargar el instalador
# https://dotnet.microsoft.com/download/dotnet/8.0
```

#### En Linux (Ubuntu/Debian)
```bash
# Agregar el repositorio
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version latest

# O usar apt
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### 2. Instalar SQL Server

#### En Windows
```bash
# Descargar desde: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
# Ejecutar el instalador

# O usar Docker (Recomendado)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=XClone@123" \
    -p 1433:1433 \
    mcr.microsoft.com/mssql/server:2022-latest
```

#### En macOS / Linux (con Docker)
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=XClone@123" \
    -p 1433:1433 \
    -d \
    mcr.microsoft.com/mssql/server:2022-latest
```

#### Verificar Conexión
```bash
# Instalar sqlcmd (si no lo tienes)
dotnet tool install -g mssql-cli

# Conectar al servidor
sqlcmd -S localhost,1433 -U sa -P XClone@123
```

### 3. Instalar MongoDB (Opcional - para logs)

```bash
# Windows: Descargar desde https://www.mongodb.com/try/download/community

# macOS
brew install mongodb-community
brew services start mongodb-community

# Linux (Ubuntu)
curl -fsSL https://www.mongodb.org/static/pgp/server-6.0.asc | sudo apt-key add -
echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/6.0 multiverse" | \
  sudo tee /etc/apt/sources.list.d/mongodb-org-6.0.list
sudo apt-get update
sudo apt-get install -y mongodb-org
sudo systemctl start mongod

# Docker (Recomendado)
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

### 4. Instalar Git

#### En Windows
```bash
# Descargar desde: https://git-scm.com/download/win
# Ejecutar el instalador
```

#### En macOS
```bash
brew install git
```

#### En Linux
```bash
sudo apt-get install -y git
```

---

## ⚙️ Configuración Inicial

### Paso 1: Clonar el Repositorio

```bash
cd ~/Documentos  # O donde guardes tus proyectos
git clone https://github.com/tu-usuario/xclone-backend.git
cd xclone-backend
```

### Paso 2: Crear Archivo .env

```bash
# Copiar el template
cp .env.example .env

# O crear manualmente
touch .env
```

### Paso 3: Configurar Variables de Entorno

Abre `.env` con tu editor favorito y configura:

```env
# ==================== BASE DE DATOS ====================
# SQL Server Connection String
# Formato: Server=<host>;Database=<database>;User Id=<user>;Password=<password>;
DefaultConnection=Server=localhost,1433;Database=XCloneDb;User Id=sa;Password=XClone@123;TrustServerCertificate=true;

# ==================== JWT ====================
# Clave secreta (MÍNIMO 32 caracteres, usa caracteres aleatorios)
# Generar: https://www.uuidgenerator.net/ o usar una frase larga
JWT:Key=tu-clave-secreta-super-larga-minimo-32-caracteres-aleatorios-1234567890
JWT:Issuer=xclone-backend
JWT:Audience=xclone-client
JWT:ExpireMinutes=60

# ==================== MONGODB (Logging) ====================
MongoDb:ConnectionString=mongodb://localhost:27017

# ==================== CLOUDINARY (Almacenamiento) ====================
# Registrate en: https://cloudinary.com
Cloudinary:CloudName=tu-cloud-name
Cloudinary:ApiKey=tu-api-key
Cloudinary:ApiSecret=tu-api-secret
```

⚠️ **IMPORTANTE:**
- **NUNCA** commitear `.env` (ya está en `.gitignore`)
- **NUNCA** compartir el `.env` por chat/email
- **NUNCA** usar claves de producción en desarrollo

### Paso 4: Restaurar Dependencias

```bash
# Desde la raíz del proyecto
dotnet restore
```

Esto descargará todos los paquetes NuGet necesarios (~30-50 segundos).

### Paso 5: Compilar el Proyecto

```bash
# Compilar la solución completa
dotnet build

# O específico
cd X.WebApi
dotnet build
```

### Paso 6: Crear la Base de Datos

```bash
cd X.WebApi

# Aplicar migraciones de Entity Framework
dotnet ef database update
```

Esto creará todas las tablas automáticamente.

#### Verificar que funcionó

```bash
# En SQL Server Management Studio o sqlcmd
SELECT name FROM sys.databases WHERE name='XCloneDb';
```

Deberías ver la base de datos `XCloneDb`.

---

## ▶️ Ejecutar en Desarrollo

### Opción 1: Línea de Comandos (Recomendado)

```bash
cd X.WebApi
dotnet run
```

Salida esperada:
```
info: Microsoft.AspNetCore.Hosting.Libuv
      Listening on https://localhost:5001
```

### Opción 2: Visual Studio Code

```bash
# Instalar extensión de C# (si no la tienes)
# Extensions > C# (by Microsoft)

# Presionar F5 o Ctrl+Shift+D para depurar
```

### Opción 3: Visual Studio 2022

```bash
# 1. Abrir X.slnx
# 2. Click derecho > Set as Startup Project
# 3. Presionar F5
```

---

## 🧪 Verificación de Setup

Ejecuta estos comandos para verificar que todo está bien:

```bash
# 1. Verificar .NET
dotnet --version  # Debe mostrar 8.0+

# 2. Verificar SQL Server
sqlcmd -S localhost,1433 -U sa -P XClone@123 -Q "SELECT @@VERSION;"

# 3. Verificar MongoDB (si lo instalaste)
mongo --version

# 4. Clonar y compilar
dotnet build

# 5. Ejecutar tests (cuando estén creados)
dotnet test

# 6. Ejecutar la aplicación
cd X.WebApi
dotnet run
```

Una vez que veas el servidor corriendo, abre en el navegador:

```
https://localhost:5001/
```

---

## 🔍 Verificación de Endpoints

### 1. Health Check (Si está implementado)

```bash
curl https://localhost:5001/health
```

### 2. Login Test

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123"}' \
  --insecure  # Para ignorar certificado auto-firmado en desarrollo
```

### 3. Ver Logs

Revisa la consola donde ejecutaste `dotnet run`

---

## ⚠️ Troubleshooting

### Problema: "Connection refused" en SQL Server

**Solución:**

```bash
# Verificar si SQL Server está corriendo
sqlcmd -S localhost,1433 -U sa

# Si falla, iniciar SQL Server
# En Windows: SQL Server Configuration Manager
# En Docker:
docker ps  # Ver si el contenedor está corriendo
docker start mssql-server  # Si está parado

# Verificar connection string en .env
# Debe ser: Server=localhost,1433;Database=...
```

### Problema: "Entity Framework migration failed"

**Solución:**

```bash
# Eliminar migraciones anteriores (si aplica)
dotnet ef migrations list
dotnet ef migrations remove

# Rehacer la migración
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Problema: "CLI error SSL/TLS certificate"

**Solución 1 - Ignorar en desarrollo:**

```bash
# En .env, cambiar:
DefaultConnection=Server=localhost,1433;...;TrustServerCertificate=true;
```

**Solución 2 - Instalar certificado auto-firmado:**

```bash
dotnet dev-certs https --trust
```

### Problema: "Port 5001 already in use"

**Solución:**

```bash
# Encontrar qué usa el puerto
# Windows
netstat -ano | findstr :5001

# Linux/macOS
lsof -i :5001

# Matar el proceso
kill -9 <PID>

# O usar un puerto diferente
dotnet run --urls https://localhost:5002
```

### Problema: "JWT Key is not configured"

**Solución:**

```bash
# Verificar que .env existe y está cargado
cat .env | grep JWT:Key

# Asegúrate que está en X.Infrastructure/.env
# Y que tiene al menos 32 caracteres
```

### Problema: "No se conecta a MongoDB"

**Solución:**

```bash
# Verificar que MongoDB está corriendo
docker ps | grep mongo

# Si no está, iniciarlo
docker run -d -p 27017:27017 --name mongodb mongo:latest

# Verificar connection string en .env
# mongodb://localhost:27017
```

---

## 📊 Scripts Útiles

Crea un archivo `scripts/dev-setup.sh` para automatizar:

```bash
#!/bin/bash

echo "🚀 Setup XClone Backend"

# 1. Crear .env si no existe
if [ ! -f ".env" ]; then
    cp .env.example .env
    echo "✅ Archivo .env creado"
fi

# 2. Restaurar dependencias
echo "📥 Restaurando dependencias..."
dotnet restore

# 3. Compilar
echo "🔨 Compilando proyecto..."
dotnet build

# 4. Migraciones
echo "🗄️ Aplicando migraciones..."
cd X.WebApi
dotnet ef database update
cd ..

echo "✅ Setup completado"
echo "Para ejecutar: cd X.WebApi && dotnet run"
```

Uso:

```bash
chmod +x scripts/dev-setup.sh
./scripts/dev-setup.sh
```

---

## 🎯 Checklist de Verificación

- [ ] .NET 8 SDK instalado
- [ ] SQL Server corriendo (local o Docker)
- [ ] Git configurado
- [ ] Repositorio clonado
- [ ] `.env` creado con valores correctos
- [ ] `dotnet restore` ejecutado
- [ ] `dotnet build` sin errores
- [ ] Migraciones aplicadas (`dotnet ef database update`)
- [ ] `dotnet run` ejecutándose sin errores
- [ ] Endpoints respondiendo ✅

---

## 📞 ¿Necesitas Ayuda?

Si algo no funciona:

1. **Revisa la sección Troubleshooting** arriba
2. **Revisa los logs** en la consola
3. **Verifica las variables de entorno**
4. **Abre un issue** en GitHub con:
   - Tu sistema operativo
   - Versión de .NET (`dotnet --version`)
   - El error exacto (con stack trace)

---

**¡Listo! Ya puedes comenzar a desarrollar.** 🎉
