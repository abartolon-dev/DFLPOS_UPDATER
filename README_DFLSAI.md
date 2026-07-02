# Conexion DFLSAI y entidades SITES / MA_CODE

Se agrego una segunda conexion llamada `DFLSAIEntities` para consultar la base existente `DFLSAI` en el servidor `ABARTOLON`.

## Conexion agregada

En `DflPosUpdater.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DFLSAIEntities": "Server=ABARTOLON;Database=DFLSAI;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Application Name=EntityFramework"
}
```

Tambien se dejo guardada la cadena legacy tipo EDMX como referencia:

```json
"DFLSAIEntities_EdmxLegacy": "metadata=res://*/FloridoERPModel.csdl|..."
```

Importante: el proyecto actual es ASP.NET Core / EF Core. Por eso se usa la parte `provider connection string` de tu cadena original, no la cadena completa `metadata=res://...`, porque esa cadena es de Entity Framework clasico con EDMX.

## Archivos relacionados

```text
DflPosUpdater.App.DAL/Data/DFLSAI/DflSaiDbContext.cs
DflPosUpdater.App.Entities/Models/DFLSAI/Site.cs
DflPosUpdater.App.Entities/Models/DFLSAI/MaCode.cs
ScriptsDatabase/RegenerarEntidadesDFLSAI.ps1
ScriptsDatabase/RegenerarEntidadesDFLSAI.bat
```

## Registro de la capa DAL

El `DbContext` se registra desde:

```text
DflPosUpdater.App.DAL/DependencyInjection.cs
```

```csharp
builder.Services.AddDataAccess(builder.Configuration);
```

Ese metodo registra ambos contextos:

```text
AppDbContext
DflSaiDbContext
```

## Regenerar entidades exactas desde SQL Server

Como el servidor `ABARTOLON` es local a tu red/equipo, las entidades incluidas son temporales. Para generar las propiedades reales de `SITES` y `MA_CODE`, ejecuta desde la raiz de la solucion:

```powershell
.\ScriptsDatabase\RegenerarEntidadesDFLSAI.ps1
```

O con CMD:

```cmd
ScriptsDatabase\RegenerarEntidadesDFLSAI.bat
```

El script ejecuta internamente un scaffold hacia capas separadas:

```bash
dotnet ef dbcontext scaffold "Server=ABARTOLON;Database=DFLSAI;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Application Name=EntityFramework" Microsoft.EntityFrameworkCore.SqlServer --project DflPosUpdater.App.DAL/DflPosUpdater.App.DAL.csproj --startup-project DflPosUpdater.Web/DflPosUpdater.Web.csproj --context DflSaiDbContext --context-dir Data/DFLSAI --output-dir ../DflPosUpdater.App.Entities/Models/DFLSAI --namespace DflPosUpdater.App.Entities.DFLSAI --context-namespace DflPosUpdater.App.DAL.DFLSAI --table SITES --table MA_CODE --force --no-onconfiguring --data-annotations
```

## Nota para migraciones

Ahora el proyecto tiene dos DbContext:

```text
AppDbContext       -> Base propia DflPosUpdater
DflSaiDbContext    -> Base existente DFLSAI, solo consulta/scaffold
```

Cuando hagas migraciones del sistema Updater, especifica el contexto correcto:

```bash
dotnet ef migrations add NombreMigracion --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
dotnet ef database update --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
```

No ejecutes migraciones sobre `DflSaiDbContext`.
