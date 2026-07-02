using DflPosUpdater.App.Entities;

namespace DflPosUpdater.App.BLL.Services.Deploy;

public interface IDeployService
{
    Task<List<Despliegue>> ListarDesplieguesAsync(int? versionId = null, int take = 200, CancellationToken cancellationToken = default);
    Task<Despliegue?> ObtenerDespliegueAsync(int id, CancellationToken cancellationToken = default);
    Task<List<DespliegueLog>> ObtenerLogsAsync(int despliegueId, CancellationToken cancellationToken = default);
    Task<List<Despliegue>> PublicarVersionAsync(int versionId, IEnumerable<int>? sucursalesIds = null, CancellationToken cancellationToken = default);
    Task<string> ReintentarDespliegueAsync(int despliegueId, CancellationToken cancellationToken = default);
    Task CancelarDespliegueAsync(int despliegueId, CancellationToken cancellationToken = default);
}
