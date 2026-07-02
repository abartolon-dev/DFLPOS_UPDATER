using DflPosUpdater.App.DAL;
using DflPosUpdater.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.BLL.Services.Updater;

public class UpdaterManifestService : IUpdaterManifestService
{
    private readonly AppDbContext _db;

    public UpdaterManifestService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UpdaterManifestResponse> ObtenerManifestAsync(string codigoSucursal, string? versionActual = null, CancellationToken cancellationToken = default)
    {
        var sucursal = await _db.Sucursales.FirstOrDefaultAsync(x => x.Codigo == codigoSucursal && x.Activa, cancellationToken);
        if (sucursal is null)
        {
            return new UpdaterManifestResponse
            {
                SucursalEncontrada = false,
                Message = "Sucursal no encontrada o inactiva."
            };
        }

        sucursal.UltimaConexion = DateTime.Now;
        if (!string.IsNullOrWhiteSpace(versionActual))
        {
            sucursal.UltimaVersionInstalada = versionActual;
        }

        var despliegue = await _db.Despliegues
            .Include(x => x.VersionApp)
                .ThenInclude(x => x!.Archivos)
            .Where(x => x.SucursalId == sucursal.Id &&
                        x.Estado == DespliegueEstado.Exitoso &&
                        x.VersionApp!.Estado == VersionEstado.Publicada)
            .OrderByDescending(x => x.VersionApp!.FechaPublicacion)
            .FirstOrDefaultAsync(cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        if (despliegue?.VersionApp is null)
        {
            return new UpdaterManifestResponse
            {
                UpdateAvailable = false,
                Message = "No hay version publicada para esta sucursal."
            };
        }

        var version = despliegue.VersionApp;
        var archivos = version.Archivos
            .Where(x => x.Activo)
            .OrderBy(x => x.RutaRelativa)
            .Select(x => new UpdaterManifestArchivoResponse
            {
                NombreArchivo = x.NombreArchivo,
                RutaRelativa = x.RutaRelativa,
                TamanoBytes = x.TamanoBytes,
                Sha256 = x.Sha256
            })
            .ToList();

        return new UpdaterManifestResponse
        {
            UpdateAvailable = !string.Equals(versionActual, version.NumeroVersion, StringComparison.OrdinalIgnoreCase),
            Version = version.NumeroVersion,
            Descripcion = version.Descripcion,
            Sha256 = version.Sha256,
            TamanoBytes = version.TamanoBytes,
            TotalArchivos = version.TotalArchivos,
            CarpetaVersion = version.NumeroVersion.Replace(' ', '_'),
            RutaRemotaCarpeta = despliegue.RutaRemotaCarpeta ?? despliegue.RutaRemotaZip,
            RutaRemotaManifest = despliegue.RutaRemotaManifest,
            FechaPublicacion = version.FechaPublicacion,
            Archivos = archivos,
            Archivo = version.NombreArchivoOriginal,
            RutaRemotaZip = despliegue.RutaRemotaZip
        };
    }
}
