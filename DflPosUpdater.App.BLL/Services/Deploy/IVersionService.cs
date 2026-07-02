using DflPosUpdater.App.Entities;
using Microsoft.AspNetCore.Http;

namespace DflPosUpdater.App.BLL.Services.Deploy;

public interface IVersionService
{
    Task<List<VersionApp>> ListarVersionesAsync(CancellationToken cancellationToken = default);
    Task<VersionApp> CrearVersionAsync(CrearVersionRequest request, CancellationToken cancellationToken = default);
    Task<VersionApp?> ObtenerVersionAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Despliegue>> ObtenerDesplieguesPorVersionAsync(int versionId, CancellationToken cancellationToken = default);
    string ObtenerRutaFisicaCarpeta(VersionApp version);
    Task<List<VersionArchivo>> AgregarArchivosAsync(
        int versionId,
        IEnumerable<IFormFile> archivos,
        string? subcarpetaBase,
        bool sobrescribirExistentes,
        CancellationToken cancellationToken = default);
    Task<int> EliminarArchivoAsync(int archivoId, CancellationToken cancellationToken = default);
}
