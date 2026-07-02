using System.Text.Json;
using DflPosUpdater.App.Entities;

namespace DflPosUpdater.App.BLL.Services.Notifications;

public class NotificationService : INotificationService
{
    public string CrearManifestJson(VersionApp version, IEnumerable<VersionArchivo> archivos, string rutaRemotaCarpeta)
    {
        var carpetaVersion = version.NumeroVersion.Replace(' ', '_');
        var listaArchivos = archivos
            .Where(x => x.Activo)
            .OrderBy(x => x.RutaRelativa)
            .Select(x => new ManifestArchivoVersion
            {
                NombreArchivo = x.NombreArchivo,
                RutaRelativa = x.RutaRelativa.Replace('\\', '/'),
                TamanoBytes = x.TamanoBytes,
                Sha256 = x.Sha256
            })
            .ToList();

        var manifest = new ManifestVersion
        {
            VersionAppId = version.Id,
            NumeroVersion = version.NumeroVersion,
            Descripcion = version.Descripcion,
            CarpetaVersion = carpetaVersion,
            RutaRemotaCarpeta = rutaRemotaCarpeta,
            TotalArchivos = listaArchivos.Count,
            TamanoBytes = listaArchivos.Sum(x => x.TamanoBytes),
            Sha256 = version.Sha256,
            FechaPublicacion = (version.FechaPublicacion ?? DateTime.Now).ToString("O"),
            Archivos = listaArchivos,

            // Compatibilidad con clientes antiguos que esperan estas propiedades.
            Archivo = carpetaVersion,
            RutaRemotaZip = rutaRemotaCarpeta
        };

        return JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}
