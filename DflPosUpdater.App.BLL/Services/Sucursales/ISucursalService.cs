using DflPosUpdater.App.Entities;

namespace DflPosUpdater.App.BLL.Services.Sucursales;

public interface ISucursalService
{
    Task<List<Sucursal>> ListarAsync(CancellationToken cancellationToken = default);
    Task<List<Sucursal>> ListarActivasAsync(CancellationToken cancellationToken = default);
    Task<Sucursal?> ObtenerAsync(int id, CancellationToken cancellationToken = default);
    Task CrearAsync(Sucursal model, CancellationToken cancellationToken = default);
    Task ActualizarAsync(int id, Sucursal model, CancellationToken cancellationToken = default);
    Task CambiarEstadoAsync(int id, CancellationToken cancellationToken = default);
}
