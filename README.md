# DflPosUpdater

Consola web ASP.NET Core MVC por capas para publicar versiones de `dflpos.exe` hacia sucursales via FTP/FTPS usando Hangfire.


## Estructura por capas

```text
DflPosUpdater.Web           -> Presentacion MVC, Views, Controllers, wwwroot
DflPosUpdater.App.BLL       -> Servicios de negocio, Jobs, FTP, manifest
DflPosUpdater.App.DAL       -> DbContext EF Core y acceso a datos
DflPosUpdater.App.Entities  -> Entidades compartidas
```

Mas detalle en `README_CAPAS.md`.

## Cambio importante de este paquete

El flujo ya no sube un ZIP a las sucursales.

Ahora el sistema crea una carpeta por version:

```text
/uploads/releases/1.0.26/
```

Y al desplegar sube al FTP de cada sucursal:

```text
/updates/releases/1.0.26/dflpos.exe
/updates/releases/1.0.26/archivo.dll
/updates/releases/1.0.26/config/appsettings.json
/updates/releases/1.0.26/manifest.json
/updates/manifest.json
```

Esto encaja mejor con un `updater.sh` que espera una carpeta con el numero de version y sus archivos.

## Pantallas principales

```text
/                         Dashboard
/Versiones                Versiones
/Versiones/Crear          Crear version y subir archivos
/ConfiguracionArchivos    Configurar archivos por version
/Sucursales               Sucursales FTP
/Despliegues              Estado de despliegues
/hangfire                 Dashboard tecnico Hangfire
/api/updater/manifest/{codigoSucursal}
```

## Base de datos

Si es una base nueva:

```bash
dotnet ef migrations add InitialCreate --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
dotnet ef database update --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
```

Si ya tenias la version anterior funcionando, puedes hacer una migracion normal:

```bash
dotnet ef migrations add FolderBasedDeployment --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
dotnet ef database update --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
```

Tambien deje un script manual:

```text
SQL_MIGRACION_ARCHIVOS.sql
```

## Flujo de uso

1. Crear o editar sucursales.
2. Crear version, por ejemplo `1.0.26`.
3. Subir los archivos que debe contener la carpeta de version.
4. Opcional: entrar a `Configuracion archivos` para agregar/reemplazar archivos.
5. Publicar la version a sucursales.
6. Hangfire crea un job por sucursal.
7. Cada job sube la carpeta `/updates/releases/{version}/` al FTP.
8. Se actualiza `/updates/manifest.json` para que la sucursal sepa que hay version nueva.

## Manifest generado

Ejemplo:

```json
{
  "versionAppId": 1,
  "numeroVersion": "1.0.26",
  "descripcion": "Correcciones POS",
  "carpetaVersion": "1.0.26",
  "rutaRemotaCarpeta": "/updates/releases/1.0.26",
  "totalArchivos": 3,
  "tamanoBytes": 123456,
  "sha256": "hash-general",
  "fechaPublicacion": "2026-05-13T10:30:00.0000000",
  "archivos": [
    {
      "nombreArchivo": "dflpos.exe",
      "rutaRelativa": "dflpos.exe",
      "tamanoBytes": 100000,
      "sha256": "hash-del-archivo"
    }
  ]
}
```

## Notas

- `RutaZip` y `RutaRemotaZip` se conservan en el modelo solo por compatibilidad con la primera version del proyecto.
- Para produccion, protege `/hangfire` y cifra `PasswordFtp`.
- Si tu FTP no permite sobrescribir archivos abiertos, el `updater.sh` debe copiar desde la carpeta de version cuando el POS no este en ejecucion.

## Librerias frontend agregadas

El proyecto ya carga estas librerias desde CDN en `Views/Shared/_Layout.cshtml`:

```text
- jQuery 3.7.1
- DataTables 2.3.8 con Bootstrap 5
- iCheck 1.0.3, skin square/blue
- SweetAlert2 11
- jQuery Validate 1.21.0
- jQuery Validation Unobtrusive 4.0.0
```

Inicializacion incluida en `wwwroot/js/site.js`:

```text
- Toda tabla con clase .js-datatable se convierte automaticamente en DataTable.
- Los checkbox/radio con clase .form-check-input se inicializan con iCheck.
- TempData["Success"] y TempData["Error"] se muestran con SweetAlert2.
- Los forms con data-confirm="true" usan confirmacion con SweetAlert2.
- jQuery Validate queda configurado con clases de Bootstrap.
```

Para usar DataTables en una tabla nueva:

```html
<table class="table table-hover js-datatable">
```

Para usar confirmacion SweetAlert2 en un formulario:

```html
<form method="post"
      data-confirm="true"
      data-confirm-title="Confirmar accion"
      data-confirm-text="Esta accion no se puede deshacer."
      data-confirm-button="Si, continuar">
```

> Nota: como las librerias se cargan por CDN, las estaciones que naveguen la consola web necesitan acceso a Internet o a esos dominios. Si prefieres instalarlas localmente en `wwwroot/lib`, se puede cambiar el layout a rutas locales.

## Scripts por vista

Los scripts especificos de cada pantalla se ubican en `wwwroot/Scripts/<NombreVista>/`.

Ejemplo incluido:

```text
wwwroot/Scripts/Sucursales/Index.js
```

La vista `Views/Sucursales/Index.cshtml` carga este archivo desde su seccion `Scripts` e inicializa el DataTable de la tabla `#tblSucursales`.


## Conexion DFLSAI

Se agrego una segunda conexion para la base existente `DFLSAI`:

```text
ConnectionStrings:DFLSAIEntities
```

Archivos relacionados:

```text
DflPosUpdater.App.DAL/Data/DFLSAI/DflSaiDbContext.cs
DflPosUpdater.App.Entities/Models/DFLSAI/Site.cs
DflPosUpdater.App.Entities/Models/DFLSAI/MaCode.cs
ScriptsDatabase/RegenerarEntidadesDFLSAI.ps1
ScriptsDatabase/RegenerarEntidadesDFLSAI.bat
README_DFLSAI.md
```

Para generar las entidades exactas de las tablas `SITES` y `MA_CODE` desde tu SQL Server `ABARTOLON`, ejecuta desde la raiz de la solucion:

```powershell
.\ScriptsDatabase\RegenerarEntidadesDFLSAI.ps1
```

Como ahora hay dos DbContext, las migraciones del Updater deben ejecutarse con:

```bash
dotnet ef migrations add NombreMigracion --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
dotnet ef database update --project DflPosUpdater.App.DAL --startup-project DflPosUpdater.Web --context AppDbContext
```
