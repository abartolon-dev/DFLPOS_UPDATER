using DflPosUpdater.App.DAL;
using DflPosUpdater.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.BLL.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardResumenDto> ObtenerResumenAsync(CancellationToken cancellationToken = default)
    {
        var versionPublicada = await _db.Versiones
            .Where(x => x.Estado == VersionEstado.Publicada)
            .OrderByDescending(x => x.FechaPublicacion)
            .FirstOrDefaultAsync(cancellationToken);

        return new DashboardResumenDto
        {
            VersionPublicada = versionPublicada,
            TotalSucursalesActivas = await _db.Sucursales.CountAsync(x => x.Activa, cancellationToken),
            TotalVersiones = await _db.Versiones.CountAsync(cancellationToken),
            DesplieguesPendientes = await _db.Despliegues.CountAsync(x => x.Estado == DespliegueEstado.Pendiente, cancellationToken),
            DesplieguesEnProceso = await _db.Despliegues.CountAsync(x => x.Estado == DespliegueEstado.EnProceso || x.Estado == DespliegueEstado.Reintentando, cancellationToken),
            DesplieguesExitosos = await _db.Despliegues.CountAsync(x => x.Estado == DespliegueEstado.Exitoso, cancellationToken),
            DesplieguesFallidos = await _db.Despliegues.CountAsync(x => x.Estado == DespliegueEstado.Fallido, cancellationToken),
            DesplieguesRecientes = await _db.Despliegues
                .Include(x => x.VersionApp)
                .Include(x => x.Sucursal)
                .OrderByDescending(x => x.FechaCreacion)
                .Take(20)
                .ToListAsync(cancellationToken)
        };
    }
}
