using System.Net;
using System.Text;
using DflPosUpdater.App.DAL;
using DflPosUpdater.App.BLL.Helpers;
using DflPosUpdater.App.Entities;
using DflPosUpdater.App.BLL.Services.Deploy;
using DflPosUpdater.App.BLL.Services.Notifications;
using FluentFTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DflPosUpdater.App.BLL.Services.FTP;

public class FtpDeployService : IFtpDeployService
{
    private readonly AppDbContext _db;
    private readonly IVersionService _versionService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<FtpDeployService> _logger;

    public FtpDeployService(
        AppDbContext db,
        IVersionService versionService,
        INotificationService notificationService,
        ILogger<FtpDeployService> logger)
    {
        _db = db;
        _versionService = versionService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task SubirVersionAsync(int despliegueId, CancellationToken cancellationToken = default)
    {
        var despliegue = await _db.Despliegues
            .Include(x => x.VersionApp)
                .ThenInclude(x => x!.Archivos)
            .Include(x => x.Sucursal)
            .FirstOrDefaultAsync(x => x.Id == despliegueId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro el despliegue {despliegueId}.");

        if (despliegue.Estado == DespliegueEstado.Cancelado || despliegue.Estado == DespliegueEstado.Omitido)
        {
            await AddLogAsync(despliegue.Id, LogTipo.Warning, "El despliegue esta cancelado u omitido. No se ejecutara.", null, cancellationToken);
            return;
        }

        var version = despliegue.VersionApp ?? throw new InvalidOperationException("El despliegue no tiene version asociada.");
        var sucursal = despliegue.Sucursal ?? throw new InvalidOperationException("El despliegue no tiene sucursal asociada.");

        if (!sucursal.Activa)
        {
            throw new InvalidOperationException($"La sucursal {sucursal.Nombre} no esta activa.");
        }

        var archivos = version.Archivos
            .Where(x => x.Activo)
            .OrderBy(x => x.RutaRelativa)
            .ToList();

        if (archivos.Count == 0)
        {
            throw new InvalidOperationException($"La version {version.NumeroVersion} no tiene archivos configurados.");
        }

        var carpetaFisica = _versionService.ObtenerRutaFisicaCarpeta(version);
        if (!Directory.Exists(carpetaFisica))
        {
            throw new DirectoryNotFoundException($"No se encontro la carpeta local de la version: {carpetaFisica}");
        }

        despliegue.Estado = DespliegueEstado.EnProceso;
        despliegue.Intentos += 1;
        despliegue.FechaInicio = DateTime.Now;
        despliegue.FechaFin = null;
        despliegue.MensajeError = null;
        await _db.SaveChangesAsync(cancellationToken);

        await AddLogAsync(despliegue.Id, LogTipo.Info, $"Iniciando subida FTP por carpeta. Intento {despliegue.Intentos}.", null, cancellationToken);

        var remoteBase = NormalizeRemotePath(sucursal.RutaDestino);
        var remoteVersionDir = $"{remoteBase}/releases/{SafeRemoteSegment(version.NumeroVersion)}";
        var remoteManifestPath = $"{remoteBase}/manifest.json";
        var remoteVersionManifestPath = $"{remoteVersionDir}/manifest.json";

        try
        {
            var config = new FtpConfig
            {
                ConnectTimeout = 30000,
                ReadTimeout = 30000,
                DataConnectionConnectTimeout = 30000
            };

            await using var client = new AsyncFtpClient(
                sucursal.HostFtp,
                new NetworkCredential(sucursal.UsuarioFtp, sucursal.PasswordFtp),
                sucursal.PuertoFtp,
                config);

            if (sucursal.UsarFtps)
            {
                client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                client.Config.ValidateAnyCertificate = true;
            }

            await AddLogAsync(despliegue.Id, LogTipo.Info, $"Conectando a FTP {sucursal.HostFtp}:{sucursal.PuertoFtp}.", null, cancellationToken);
            await client.AutoConnect(cancellationToken);

            await AddLogAsync(despliegue.Id, LogTipo.Info, $"Subiendo {archivos.Count} archivo(s) a {remoteVersionDir}.", null, cancellationToken);

            foreach (var archivo in archivos)
            {
                var localPath = GetSafeLocalPath(carpetaFisica, archivo.RutaRelativa);
                if (!File.Exists(localPath))
                {
                    throw new FileNotFoundException($"No se encontro el archivo local {archivo.RutaRelativa}.", localPath);
                }

                var remoteFilePath = $"{remoteVersionDir}/{archivo.RutaRelativa.Replace('\\', '/')}";
                var uploadStatus = await client.UploadFile(
                    localPath,
                    remoteFilePath,
                    FtpRemoteExists.Overwrite,
                    createRemoteDir: true,
                    verifyOptions: FtpVerify.Retry,
                    progress: null,
                    token: cancellationToken);

                if (uploadStatus != FtpStatus.Success)
                {
                    throw new InvalidOperationException($"FluentFTP reporto estado {uploadStatus} al subir {archivo.RutaRelativa}.");
                }
            }

            var manifestJson = _notificationService.CrearManifestJson(version, archivos, remoteVersionDir);
            var manifestBytes = Encoding.UTF8.GetBytes(manifestJson);

            await AddLogAsync(despliegue.Id, LogTipo.Info, $"Subiendo manifest de version a {remoteVersionManifestPath}.", null, cancellationToken);
            await client.UploadBytes(
                manifestBytes,
                remoteVersionManifestPath,
                FtpRemoteExists.Overwrite,
                createRemoteDir: true,
                progress: null,
                token: cancellationToken);

            await AddLogAsync(despliegue.Id, LogTipo.Info, $"Actualizando manifest activo en {remoteManifestPath}.", null, cancellationToken);
            await client.UploadBytes(
                manifestBytes,
                remoteManifestPath,
                FtpRemoteExists.Overwrite,
                createRemoteDir: true,
                progress: null,
                token: cancellationToken);

            await client.Disconnect(cancellationToken);

            despliegue.Estado = DespliegueEstado.Exitoso;
            despliegue.FechaFin = DateTime.Now;
            despliegue.FechaNotificacion = DateTime.Now;
            despliegue.RutaRemotaCarpeta = remoteVersionDir;
            despliegue.RutaRemotaZip = remoteVersionDir;
            despliegue.RutaRemotaManifest = remoteManifestPath;
            despliegue.MensajeError = null;

            await _db.SaveChangesAsync(cancellationToken);
            await AddLogAsync(despliegue.Id, LogTipo.Success, "Carpeta de version subida y manifest actualizado correctamente.", null, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desplegar version {Version} en sucursal {Sucursal}", version.NumeroVersion, sucursal.Nombre);

            despliegue.Estado = DespliegueEstado.Fallido;
            despliegue.FechaFin = DateTime.Now;
            despliegue.MensajeError = ex.Message;
            await _db.SaveChangesAsync(cancellationToken);

            await AddLogAsync(despliegue.Id, LogTipo.Error, "Error al subir carpeta de version por FTP.", ex.ToString(), cancellationToken);

            // Importante: se relanza para que Hangfire registre el fallo y aplique sus reintentos automaticos.
            throw;
        }
    }

    private async Task AddLogAsync(int despliegueId, LogTipo tipo, string mensaje, string? detalle, CancellationToken cancellationToken)
    {
        _db.DespliegueLogs.Add(new DespliegueLog
        {
            DespliegueId = despliegueId,
            Tipo = tipo,
            Mensaje = mensaje,
            Detalle = detalle,
            Fecha = DateTime.Now
        });

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static string GetSafeLocalPath(string baseDirectory, string relativePath)
    {
        var fullBase = Path.GetFullPath(baseDirectory);
        var fullPath = Path.GetFullPath(Path.Combine(fullBase, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        var normalizedBase = fullBase.EndsWith(Path.DirectorySeparatorChar)
            ? fullBase
            : fullBase + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Ruta local de archivo invalida.");
        }

        return fullPath;
    }

    private static string NormalizeRemotePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "/updates";
        }

        var normalized = path.Replace('\\', '/').Trim();
        if (!normalized.StartsWith('/'))
        {
            normalized = "/" + normalized;
        }

        return normalized.TrimEnd('/');
    }

    private static string SafeRemoteSegment(string value)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalid, '_');
        }

        return value.Replace(' ', '_');
    }
}
