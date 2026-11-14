# Rótulos Ranibal — Fase 3

Proyecto ASP.NET Core MVC con C#, .NET 10, Entity Framework Core 10 y SQL Server.

## Estructura

- `Controllers/`: controladores MVC.
- `Models/`: entidades del dominio.
- `Data/`: `ApplicationDbContext` y Unit of Work.
- `Repositories/`: interfaces e implementaciones de acceso a datos.
- `Services/`: reglas de negocio y consultas de informes.
- `ViewModels/`: modelos de presentación.
- `Views/`: vistas Razor del proyecto principal.
- `Migrations/`: migraciones de Entity Framework Core.
- `PruebasPago/`: pruebas funcionales independientes del módulo de Pagos e Informes.

Se consolidó el módulo de Pagos e Informes en el único proyecto MVC de la raíz. Ya no se necesita abrir un segundo `.csproj` anidado.

## Requisitos

- .NET SDK 10.
- SQL Server LocalDB o una instancia SQL Server accesible.
- Herramientas de Entity Framework Core (`dotnet-ef`) para administrar migraciones.

## Configuración

Revisar `appsettings.json` y ajustar `ConnectionStrings:DefaultConnection` si la instancia SQL Server local utiliza otro nombre.

Cadena de conexión incluida para LocalDB:

```text
Server=(localdb)\mssqllocaldb;Database=DAS_Proyecto_DB;Trusted_Connection=True;MultipleActiveResultSets=true
```

## Compilar la aplicación

Desde la carpeta donde está `DAS Proyecto.csproj`:

```bash
dotnet restore "DAS Proyecto.csproj"
dotnet build "DAS Proyecto.csproj"
```

## Aplicar migraciones

Antes de aplicar migraciones sobre una base de datos que ya contenga información, realizar una copia de seguridad y comprobar que las relaciones existentes no tengan registros huérfanos.

Después, ejecutar:

```bash
dotnet ef database update --project "DAS Proyecto.csproj"
```

La migración `20261114110000_CorreccionRelaciones` elimina las columnas de claves foráneas sombra creadas por `AjusteModelo`, vuelve a vincular las relaciones con los campos `Id...` declarados en las entidades y establece la relación `Pago`–`OrdenTrabajo`. También crea índices para códigos de pago únicos y para impedir más de un anticipo por orden.

Si la base de datos ya fue modificada manualmente o contiene datos con relaciones inconsistentes, detenerse y revisar la información antes de aplicar la migración.

## Ejecutar la aplicación

```bash
dotnet run --project "DAS Proyecto.csproj"
```

El perfil HTTP de `Properties/launchSettings.json` usa `http://localhost:5049`.

## Ejecutar pruebas de Pagos e Informes

```bash
dotnet run --project "PruebasPago/PruebasPago.csproj"
```

Las pruebas usan un repositorio simulado: validan reglas de negocio y cálculos, pero no sustituyen pruebas de integración contra SQL Server.

## Alcance y pendientes conocidos

- Pagos e Informes ahora usan repositorio EF Core, servicio, Unit of Work, controlador y ViewModels en el proyecto principal.
- Las relaciones declaradas se configuran explícitamente en Fluent API para evitar columnas sombra duplicadas.
- Los módulos de cotizaciones, órdenes e inventario aún requieren completar sus operaciones persistentes y flujos de negocio; no se consideran terminados solo por tener vistas o controladores.
- La autenticación y autorización por roles todavía requiere implementación y pruebas completas.
- La aplicación y las migraciones deben compilarse y probarse en el equipo con .NET 10 y SQL Server antes de entregar.
