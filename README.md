# 🚀 microservices-solution

## Solución .NET 8 basada en microservicios que incluye:

- **UsersService:** registro, login JWT, autorización por roles

- **ProductsService:** CRUD de productos

- **API Gateway (Ocelot):** enrutamiento y verificación de JWT

- **BuildingBlocks:** middleware de errores (ProblemDetails), correlation id y utilidades comunes

- **Tests:** xUnit + Moq + FluentAssertions

- **Logs (Serilog):** consola y archivo diario por servicio

## 🏗️ Arquitectura del Proyecto
La solución está organizada en los siguientes proyectos:

- **BuildingBlocks:** Middlewares y utilidades compartidas (manejo global de errores, Correlation-Id, tipos de error estándar).

- **ApiGateway/Gateway:** API Gateway con Ocelot, autenticación JWT y enrutamiento.

- **UsersService**

	- **UsersService.Domain:** Entidades y enums de dominio.

	- **UsersService.Application:** DTOs, CQRS (comandos/queries/handlers), validaciones.

	- **UsersService.Infrastructure:** EF Core, repositorios, migraciones, hashing Bcrypt y JwtTokenFactory.

	- **UsersService.Web:** API ASP.NET Core (Controllers, Program, Swagger, Serilog).

- **ProductsService**

	- **ProductsService.Domain:** Entidades y enums de dominio.

	- **ProductsService.Application:** DTOs, CQRS (comandos/queries/handlers), validaciones.

	- **ProductsService.Infrastructure:** EF Core, repositorios, migraciones.

	- **ProductsService.Web:** API ASP.NET Core (Controllers, Program, Swagger, Serilog).

- **Tests

	- **UsersService.Tests:** pruebas de lógica (Application/Domain).

	- **ProductsService.Tests:** pruebas de lógica (Application/Domain).

## 📁 Estructura de Carpetas
```microservices-solution/
├─ src/
│  ├─ BuildingBlocks/
│  ├─ ApiGateway/
│  │  └─ Gateway/
│  └─ Services/
│     ├─ UsersService/
│     │  ├─ UsersService.Domain/
│     │  ├─ UsersService.Application/
│     │  ├─ UsersService.Infrastructure/
│     │  └─ UsersService.Web/
│     └─ ProductsService/
│        ├─ ProductsService.Domain/
│        ├─ ProductsService.Application/
│        ├─ ProductsService.Infrastructure/
│        └─ ProductsService.Web/
├─ tests/
│  ├─ UsersService.Tests/
│  └─ ProductsService.Tests/
└─ coverlet.runsettings   (o tests/coverlet.runsettings)
```

## 🚀 Requisitos Previos
Antes de ejecutar el proyecto, asegúrate de tener instalado lo siguiente:
- Visual Studio 2022 con el paquete de desarrollo de ASP.NET y desarrollo de bases de datos.
- .NET SDK 8.0
- PostgreSQL 13+ (local)
- dotnet-ef para aplicar migraciones (CLI):
	```
	dotnet tool install -g dotnet-ef
	```
- (Opcional) reportgenerator para ver cobertura en HTML:
	```
	dotnet tool install -g dotnet-reportgenerator-globaltool
	```

## 🚀 Instalación y Ejecución

## 1. 📥 Clonación del Repositorio
Para clonar el repositorio, ejecuta el siguiente comando en tu terminal:
```bash
git clone https://github.com/rony1589/microservicesNetAuth.git
cd microservicesNetAuth
```

## 2. Restaurar los paquetes NuGet

dotnet restore

Compilar:

dotnet build

## 3. ⚙️ Configuración de la Conexión a la Base de Datos
La cadena de conexión a la base de datos se encuentra en el archivo appsettings.json dentro de los difernetes proyectos.

### Reemplaza Host, Port, Username con el nombre y Password de tu Servidor de PostgreSQL.
📁 src/Services/UsersService/UsersService.Web/appsettings.json
```
{
  "ConnectionStrings": {
    "UsersDb": "Host=localhost;Port=5432;Database=users_db;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "microservices-solution",
    "Audience": "microservices-solution",
    "SigningKey": "nwOpxyBSuiZPb7Cjv5wFfFIA1vHFiTM8"
  },
  "Serilog": {
    "MinimumLevel": { "Default": "Information" },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/users-service-.log", "rollingInterval": "Day" } }
    ],
    "Enrich": [ "FromLogContext" ]
  },
  "AllowedHosts": "*"
}
```

📁 src/Services/ProductsService/ProductsService.Web/appsettings.json

```
{
  "ConnectionStrings": {
    "ProductsDb": "Host=localhost;Port=5432;Database=products_db;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "microservices-solution",
    "Audience": "microservices-solution",
    "SigningKey": "nwOpxyBSuiZPb7Cjv5wFfFIA1vHFiTM8"
  },
  "Serilog": {
    "MinimumLevel": { "Default": "Information" },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/products-service-.log", "rollingInterval": "Day" } }
    ],
    "Enrich": [ "FromLogContext" ]
  },
  "AllowedHosts": "*"
}

```

### Aca solo si requeires cambiar el SigningKey el cual es la clave del JWT y debe ser igual en los 3 archivos.
📁 src/ApiGateway/Gateway/appsettings.json

```
{
  "Jwt": {
    "Issuer": "microservices-solution",
    "Audience": "microservices-solution",
    "SigningKey": "nwOpxyBSuiZPb7Cjv5wFfFIA1vHFiTM8"
  },
  "Serilog": {
    "MinimumLevel": { "Default": "Information" },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/gateway-.log", "rollingInterval": "Day" } }
    ],
    "Enrich": [ "FromLogContext" ]
  }
}

```


## 🧪 Creación de las Base de Datos y Migraciones EF Core
Para aplicar las migraciones y crear la base de datos, utiliza la Consola del Administrador de Paquetes en Visual Studio:

### 1. Crear bases vacías en PostgreSQL
```
CREATE DATABASE users_db;
CREATE DATABASE products_db;
```

### 2. Aplicar migraciones (desde la raíz del repo) dado que los archivos ya existen:

dotnet build

### UsersService (aplicar migraciones a la BD)
dotnet ef database update `
  -c UsersDbContext `
  -p "src/Services/UsersService/UsersService.Infrastructure/UsersService.Infrastructure.csproj" `
  -s "src/Services/UsersService/UsersService.Web/UsersService.Web.csproj"
  
### ProductsService (aplicar migraciones a la BD)
 dotnet ef database update `
  -c ProductsDbContext `
  -p "src/Services/ProductsService/ProductsService.Infrastructure/ProductsService.Infrastructure.csproj" `
  -s "src/Services/ProductsService/ProductsService.Web/ProductsService.Web.csproj"
  
## ▶️ Usuarios Login
1. Para ingresar con un usuario de rol Admin usar el siguiente:
   Email: admin@demo.com
   Password: Admin123
   
2. Para ingresar con un usuario de rol Usuario usar el siguiente:
   Email: User123@demo.com
   Password: User123


## ▶️ Ajuste de Puertos y Ejecución (cada servicio en su consola)
En caso de que los puertos que estan documentados esten ocupados en el siguinete archivo de rutas: src/ApiGateway/Gateway/ocelot.json
podran cambiarlo y asi mismo se debera cambiar en los comandos que ejecutan cada microservicio y el Gateway


# UsersService (5001) si cambia el 
dotnet run --no-build --urls http://localhost:5001 --project src/Services/UsersService/UsersService.Web

# ProductsService (5002)
dotnet run --no-build --urls http://localhost:5002 --project src/Services/ProductsService/ProductsService.Web

# Gateway (5000)
dotnet run --no-build --urls http://localhost:5000 --project src/ApiGateway/Gateway

## 🌉 API Gateway (Ocelot)
- Público (sin token):
	POST /users/login → :5001/api/users/login
	POST /users/register → :5001/api/users/register

- Protegido (con Bearer JWT):
	GET /users/{...} y todos /products/{...}

- Flujo rápido (Postman/HTTP): siempre se debe apuntar al Gateway

	POST http://localhost:5000/users/login

	Content-Type: application/json

	{ "email": "admin@demo.com", "password": "P@ssw0rd" }



## 📜 Swagger
- Users: http://localhost:5001/swagger

- roducts: http://localhost:5002/swagger


## 📝 Logs (Serilog)
Archivos diarios (rotación automática):

- Users → src/Services/UsersService/UsersService.Web/logs/users-service-YYYYMMDD.log

- Products → src/Services/ProductsService/ProductsService.Web/logs/products-service-YYYYMMDD.log

- Gateway → src/ApiGateway/Gateway/logs/gateway-YYYYMMDD.log


## 🧪 Pruebas Unitarias y Cobertura
### Ejecutar pruebas + cobertura (desde la raíz)
```
dotnet test --settings coverlet.runsettings `
            --collect:"XPlat Code Coverage" `
            --results-directory tests/TestResults/last
```

### Generar reporte HTML (opcional)
```
reportgenerator -reports:"tests/TestResults/last/**/coverage.cobertura.xml" `
                -targetdir:"tests/coverage-report"
```

### Abrir Reporte 
```
start tests/coverage-report/index.html
```