# DflPosUpdater - Diseño por capas

El proyecto fue separado para que la capa Web no concentre toda la logica.

## Estructura

```text
DflPosUpdaterCapas/
├── DflPosUpdater.sln
├── DflPosUpdater.Web/
├── DflPosUpdater.App.BLL/
├── DflPosUpdater.App.DAL/
├── DflPosUpdater.App.Entities/
└── ScriptsDatabase/
```

## Responsabilidades

### DflPosUpdater.Web
Capa de presentacion MVC.

Contiene:

```text
Controllers/
Views/
ViewModels/
wwwroot/
Configurations/
Middleware/
Program.cs
appsettings.json
```

Aqui quedan las pantallas, validaciones visuales, DataTables, SweetAlert2, iCheck, jQuery Validate y el dashboard de Hangfire.

Los controladores ya no hacen consultas directas a EF Core. Ahora consumen servicios de BLL.

### DflPosUpdater.App.BLL
Capa de negocio.

Contiene:

```text
Services/
Jobs/
Helpers/
DependencyInjection.cs
```

Aqui vive la logica de:

```text
- Crear versiones
- Agregar/eliminar archivos de version
- Publicar despliegues
- Encolar jobs de Hangfire
- Subir carpetas por FTP
- Generar manifest.json
- Consultar manifest para updater.sh
- Administrar sucursales
- Dashboard de negocio
```

### DflPosUpdater.App.DAL
Capa de acceso a datos.

Contiene:

```text
Data/AppDbContext.cs
Data/DFLSAI/DflSaiDbContext.cs
DependencyInjection.cs
```

Aqui quedan los DbContext y la configuracion EF Core hacia SQL Server.

### DflPosUpdater.App.Entities
Capa de entidades compartidas.

Contiene:

```text
Models/VersionApp.cs
Models/VersionArchivo.cs
Models/Sucursal.cs
Models/Despliegue.cs
Models/DespliegueLog.cs
Models/DFLSAI/Site.cs
Models/DFLSAI/MaCode.cs
```

Se agrego esta capa para evitar dependencias circulares entre BLL y DAL.

## Flujo de dependencias

```text
DflPosUpdater.Web
    ↓
DflPosUpdater.App.BLL
    ↓
DflPosUpdater.App.DAL
    ↓
DflPosUpdater.App.Entities
```

La capa Web tambien referencia `App.Entities` para poder usar las entidades en las vistas Razor.
`Program.cs` referencia `App.DAL` porque es el composition root donde se registra la inyeccion de dependencias.

## Inyeccion de dependencias

En `Program.cs` ahora solo se registran las capas:

```csharp
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessLayer();
```

`AddDataAccess` esta en:

```text
DflPosUpdater.App.DAL/DependencyInjection.cs
```

`AddBusinessLayer` esta en:

```text
DflPosUpdater.App.BLL/DependencyInjection.cs
```

## Comandos importantes

Restaurar y compilar:

```bash
dotnet restore DflPosUpdater.sln
dotnet build DflPosUpdater.sln
```

Ejecutar:

```bash
dotnet run --project DflPosUpdater.Web/DflPosUpdater.Web.csproj
```

Crear migraciones del sistema updater:

```bash
dotnet ef migrations add NombreMigracion --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
```

Actualizar base del sistema updater:

```bash
dotnet ef database update --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
```

Regenerar entidades de DFLSAI para SITES y MA_CODE:

```powershell
.\ScriptsDatabase\RegenerarEntidadesDFLSAI.ps1
```

## Nota

La conexion `DFLSAIEntities_EdmxLegacy` se conserva solo como referencia del connection string EDMX anterior. Para EF Core se usa `DFLSAIEntities` con formato SQL Server directo.
