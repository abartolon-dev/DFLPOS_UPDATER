using DflPosUpdater.App.DAL;
using DflPosUpdater.App.BLL.Jobs;
using DflPosUpdater.App.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.BLL.Services.Deploy;

public class DeployService : IDeployService
{
    private readonly AppDbContext _db;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public DeployService(AppDbContext db, IBackgroundJobClient backgroundJobClient)
    {
        _db = db;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<List<Despliegue>> ListarDesplieguesAsync(int? versionId = null, int take = 200, CancellationToken cancellationToken = default)
    {
        var query = _db.Despliegues
            .Include(x => x.VersionApp)
            .Include(x => x.Sucursal)
            .AsQueryable();

        if (versionId.HasValue)
        {
            query = query.Where(x => x.VersionAppId == versionId.Value);
        }

        return await query
            .OrderByDescending(x => x.FechaCreacion)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<Despliegue?> ObtenerDespliegueAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Despliegues
            .Include(x => x.VersionApp)
            .Include(x => x.Sucursal)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<DespliegueLog>> ObtenerLogsAsync(int despliegueId, CancellationToken cancellationToken = default)
    {
        return _db.DespliegueLogs
            .Where(x => x.DespliegueId == despliegueId)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Despliegue>> PublicarVersionAsync(int versionId, IEnumerable<int>? sucursalesIds = null, CancellationToken cancellationToken = default)
    {
        var version = await _db.Versiones
            .Include(x => x.Archivos)
            .FirstOrDefaultAsync(x => x.Id == versionId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro la version.");

        if (!version.Archivos.Any(x => x.Activo))
        {
            throw new InvalidOperationException("La version no tiene archivos configurados para desplegar.");
        }

        var query = _db.Sucursales.Where(x => x.Activa);
        var ids = sucursalesIds?.ToArray();
        if (ids is { Length: > 0 })
        {
            query = query.Where(x => ids.Contains(x.Id));
        }

        var sucursales = await query.OrderBy(x => x.Nombre).ToListAsync(cancellationToken);
        if (sucursales.Count == 0)
        {
            throw new InvalidOperationException("No hay sucursales activas para publicar.");
        }

        if (version.Estado == VersionEstado.Borrador)
        {
            version.Estado = VersionEstado.Publicada;
            version.FechaPublicacion = DateTime.Now;
        }

        var despliegues = new List<Despliegue>();

        foreach (var sucursal in sucursales)
        {
            var existe = await _db.Despliegues.AnyAsync(
                x => x.VersionAppId == version.Id && x.SucursalId == sucursal.Id,
                cancellationToken);

            if (existe)
            {
                continue;
            }

            despliegues.Add(new Despliegue
            {
                VersionAppId = version.Id,
                SucursalId = sucursal.Id,
                Estado = DespliegueEstado.Pendiente,
                FechaCreacion = DateTime.Now
            });
        }

        _db.Despliegues.AddRange(despliegues);
        await _db.SaveChangesAsync(cancellationToken);

        foreach (var despliegue in despliegues)
        {
            var jobId = _backgroundJobClient.Enqueue<DeploySucursalJob>(job => job.EjecutarAsync(despliegue.Id));
            despliegue.HangfireJobId = jobId;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return despliegues;
    }

    public async Task<string> ReintentarDespliegueAsync(int despliegueId, CancellationToken cancellationToken = default)
    {
        var despliegue = await _db.Despliegues.FirstOrDefaultAsync(x => x.Id == despliegueId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el despliegue.");

        despliegue.Estado = DespliegueEstado.Reintentando;
        despliegue.MensajeError = null;
        despliegue.FechaInicio = null;
        despliegue.FechaFin = null;

        var jobId = _backgroundJobClient.Enqueue<DeploySucursalJob>(job => job.EjecutarAsync(despliegue.Id));
        despliegue.HangfireJobId = jobId;

        _db.DespliegueLogs.Add(new DespliegueLog
        {
            DespliegueId = despliegue.Id,
            Tipo = LogTipo.Warning,
            Mensaje = "Reintento manual encolado desde el panel web.",
            Fecha = DateTime.Now
        });

        await _db.SaveChangesAsync(cancellationToken);
        return jobId;
    }

    public async Task CancelarDespliegueAsync(int despliegueId, CancellationToken cancellationToken = default)
    {
        var despliegue = await _db.Despliegues.FirstOrDefaultAsync(x => x.Id == despliegueId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el despliegue.");

        if (despliegue.Estado == DespliegueEstado.Exitoso)
        {
            throw new InvalidOperationException("No puedes cancelar un despliegue exitoso.");
        }

        despliegue.Estado = DespliegueEstado.Cancelado;
        despliegue.FechaFin = DateTime.Now;
        despliegue.MensajeError = "Cancelado manualmente.";

        _db.DespliegueLogs.Add(new DespliegueLog
        {
            DespliegueId = despliegue.Id,
            Tipo = LogTipo.Warning,
            Mensaje = "Despliegue cancelado manualmente.",
            Fecha = DateTime.Now
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}
