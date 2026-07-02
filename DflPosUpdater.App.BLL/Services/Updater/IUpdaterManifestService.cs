namespace DflPosUpdater.App.BLL.Services.Updater;

public interface IUpdaterManifestService
{
    Task<UpdaterManifestResponse> ObtenerManifestAsync(string codigoSucursal, string? versionActual = null, CancellationToken cancellationToken = default);
}
