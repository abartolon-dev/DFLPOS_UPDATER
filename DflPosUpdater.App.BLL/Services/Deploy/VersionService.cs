using System.Security.Cryptography;
using System.Text;
using DflPosUpdater.App.DAL;
using DflPosUpdater.App.BLL.Helpers;
using DflPosUpdater.App.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.BLL.Services.Deploy;

public class VersionService : IVersionService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;

    public VersionService(AppDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public Task<List<VersionApp>> ListarVersionesAsync(CancellationToken cancellationToken = default)
    {
        return _db.Versiones
            .Include(x => x.Archivos)
            .OrderByDescending(x => x.FechaCreacion)
            .ToListAsync(cancellationToken);
    }

    public async Task<VersionApp> CrearVersionAsync(CrearVersionRequest request, CancellationToken cancellationToken = default)
    {
        var archivos = request.Archivos?.Where(x => x.Length > 0).ToList() ?? new List<IFormFile>();
        if (archivos.Count == 0)
        {
            throw new InvalidOperationException("Debes seleccionar al menos un archivo para la version.");
        }

        var numeroVersion = request.NumeroVersion.Trim();
        if (await _db.Versiones.AnyAsync(x => x.NumeroVersion == numeroVersion, cancellationToken))
        {
            throw new InvalidOperationException($"La version {numeroVersion} ya existe.");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var safeVersion = FileHelper.ToSafeFolderName(numeroVersion);
        var carpetaWeb = FileHelper.ToWebPath("uploads", "releases", safeVersion);
        var carpetaFisica = FileHelper.ToPhysicalPath(_environment.WebRootPath, carpetaWeb);
        FileHelper.EnsureDirectory(carpetaFisica);

        var version = new VersionApp
        {
            NumeroVersion = numeroVersion,
            Descripcion = request.Descripcion?.Trim(),
            RutaCarpeta = carpetaWeb,
            RutaZip = carpetaWeb,
            NombreArchivoOriginal = safeVersion,
            TamanoBytes = 0,
            TotalArchivos = 0,
            Sha256 = string.Empty,
            Estado = VersionEstado.Borrador,
            FechaCreacion = DateTime.Now
        };

        _db.Versiones.Add(version);
        await _db.SaveChangesAsync(cancellationToken);

        await AgregarArchivosInternoAsync(version, archivos, request.SubcarpetaBase, sobrescribirExistentes: true, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return version;
    }

    public Task<VersionApp?> ObtenerVersionAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Versiones
            .Include(x => x.Archivos)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Despliegue>> ObtenerDesplieguesPorVersionAsync(int versionId, CancellationToken cancellationToken = default)
    {
        return _db.Despliegues
            .Include(x => x.Sucursal)
            .Where(x => x.VersionAppId == versionId)
            .OrderBy(x => x.Sucursal!.Nombre)
            .ToListAsync(cancellationToken);
    }

    public string ObtenerRutaFisicaCarpeta(VersionApp version)
    {
        var ruta = !string.IsNullOrWhiteSpace(version.RutaCarpeta) ? version.RutaCarpeta : version.RutaZip;
        return FileHelper.ToPhysicalPath(_environment.WebRootPath, ruta);
    }

    public async Task<List<VersionArchivo>> AgregarArchivosAsync(
        int versionId,
        IEnumerable<IFormFile> archivos,
        string? subcarpetaBase,
        bool sobrescribirExistentes,
        CancellationToken cancellationToken = default)
    {
        var version = await _db.Versiones
            .Include(x => x.Archivos)
            .FirstOrDefaultAsync(x => x.Id == versionId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro la version.");

        var archivosValidos = archivos.Where(x => x.Length > 0).ToList();
        if (archivosValidos.Count == 0)
        {
            throw new InvalidOperationException("Debes seleccionar al menos un archivo.");
        }

        return await AgregarArchivosInternoAsync(version, archivosValidos, subcarpetaBase, sobrescribirExistentes, cancellationToken);
    }

    public async Task<int> EliminarArchivoAsync(int archivoId, CancellationToken cancellationToken = default)
    {
        var archivo = await _db.VersionArchivos
            .Include(x => x.VersionApp)
            .FirstOrDefaultAsync(x => x.Id == archivoId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el archivo de version.");

        var versionId = archivo.VersionAppId;
        var version = archivo.VersionApp ?? await _db.Versiones.FirstAsync(x => x.Id == versionId, cancellationToken);
        var path = FileHelper.ToPhysicalPath(_environment.WebRootPath, archivo.RutaWeb);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        _db.VersionArchivos.Remove(archivo);
        await _db.SaveChangesAsync(cancellationToken);
        await RecalcularTotalesAsync(version, cancellationToken);

        return versionId;
    }

    private async Task<List<VersionArchivo>> AgregarArchivosInternoAsync(
        VersionApp version,
        IReadOnlyCollection<IFormFile> archivos,
        string? subcarpetaBase,
        bool sobrescribirExistentes,
        CancellationToken cancellationToken)
    {
        var carpetaFisica = ObtenerRutaFisicaCarpeta(version);
        FileHelper.EnsureDirectory(carpetaFisica);

        var guardados = new List<VersionArchivo>();
        var rutasEnSolicitud = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var archivo in archivos)
        {
            var rutaRelativa = BuildSafeRelativePath(archivo.FileName, subcarpetaBase);

            if (!rutasEnSolicitud.Add(rutaRelativa))
            {
                throw new InvalidOperationException($"El archivo {rutaRelativa} esta repetido en la carga.");
            }

            var destinoFisico = GetSafePhysicalPath(carpetaFisica, rutaRelativa);
            FileHelper.EnsureDirectory(Path.GetDirectoryName(destinoFisico)!);

            var existente = await _db.VersionArchivos
                .FirstOrDefaultAsync(x => x.VersionAppId == version.Id && x.RutaRelativa == rutaRelativa, cancellationToken);

            if (existente is not null && !sobrescribirExistentes)
            {
                throw new InvalidOperationException($"Ya existe un archivo con la ruta {rutaRelativa}.");
            }

            await using (var stream = File.Create(destinoFisico))
            {
                await archivo.CopyToAsync(stream, cancellationToken);
            }

            var sha256 = await CalcularSha256Async(destinoFisico, cancellationToken);
            var fileInfo = new FileInfo(destinoFisico);
            var nombreArchivo = Path.GetFileName(rutaRelativa);
            var rutaWeb = FileHelper.ToWebPath("uploads", "releases", FileHelper.ToSafeFolderName(version.NumeroVersion), rutaRelativa);

            if (existente is null)
            {
                existente = new VersionArchivo
                {
                    VersionAppId = version.Id,
                    RutaRelativa = rutaRelativa,
                    FechaCarga = DateTime.Now
                };

                _db.VersionArchivos.Add(existente);
            }

            existente.NombreArchivo = nombreArchivo;
            existente.RutaWeb = rutaWeb;
            existente.TamanoBytes = fileInfo.Length;
            existente.Sha256 = sha256;
            existente.Activo = true;
            existente.FechaCarga = DateTime.Now;

            guardados.Add(existente);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await RecalcularTotalesAsync(version, cancellationToken);

        return guardados;
    }

    private async Task RecalcularTotalesAsync(VersionApp version, CancellationToken cancellationToken)
    {
        var archivos = await _db.VersionArchivos
            .Where(x => x.VersionAppId == version.Id && x.Activo)
            .OrderBy(x => x.RutaRelativa)
            .ToListAsync(cancellationToken);

        version.TotalArchivos = archivos.Count;
        version.TamanoBytes = archivos.Sum(x => x.TamanoBytes);
        version.Sha256 = CalcularHashContenido(archivos);
        version.RutaZip = version.RutaCarpeta;
        version.NombreArchivoOriginal = FileHelper.ToSafeFolderName(version.NumeroVersion);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static string BuildSafeRelativePath(string fileName, string? subcarpetaBase)
    {
        var normalizedFileName = (fileName ?? string.Empty).Replace('\\', '/').Trim('/');

        // Evita guardar rutas locales tipo C:/fakepath/archivo.exe.
        if (normalizedFileName.Contains(':'))
        {
            normalizedFileName = normalizedFileName.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
        }

        string relative;
        if (!string.IsNullOrWhiteSpace(subcarpetaBase))
        {
            relative = $"{subcarpetaBase.Replace('\\', '/')}/{Path.GetFileName(normalizedFileName)}";
        }
        else
        {
            relative = normalizedFileName;
        }

        var segments = relative
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => x != "." && x != "..")
            .Select(FileHelper.ToSafeFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (segments.Length == 0)
        {
            throw new InvalidOperationException("Nombre de archivo invalido.");
        }

        return string.Join('/', segments);
    }

    private static string GetSafePhysicalPath(string baseDirectory, string relativePath)
    {
        var fullBase = Path.GetFullPath(baseDirectory);
        var fullPath = Path.GetFullPath(Path.Combine(fullBase, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        var normalizedBase = fullBase.EndsWith(Path.DirectorySeparatorChar)
            ? fullBase
            : fullBase + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Ruta de archivo invalida.");
        }

        return fullPath;
    }

    private static async Task<string> CalcularSha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string CalcularHashContenido(IEnumerable<VersionArchivo> archivos)
    {
        var builder = new StringBuilder();
        foreach (var archivo in archivos.OrderBy(x => x.RutaRelativa))
        {
            builder.Append(archivo.RutaRelativa)
                .Append('|')
                .Append(archivo.TamanoBytes)
                .Append('|')
                .Append(archivo.Sha256)
                .AppendLine();
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
