using DflPosUpdater.App.DAL;
using DflPosUpdater.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.BLL.Services.Sucursales;

public class SucursalService : ISucursalService
{
    private readonly AppDbContext _db;

    public SucursalService(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Sucursal>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return _db.Sucursales
            .OrderBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Sucursal>> ListarActivasAsync(CancellationToken cancellationToken = default)
    {
        return _db.Sucursales
            .Where(x => x.Activa)
            .OrderBy(x => x.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<Sucursal?> ObtenerAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Sucursales.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task CrearAsync(Sucursal model, CancellationToken cancellationToken = default)
    {
        LimpiarSucursal(model);

        if (await _db.Sucursales.AnyAsync(x => x.Codigo == model.Codigo, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe una sucursal con este codigo.");
        }

        _db.Sucursales.Add(model);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ActualizarAsync(int id, Sucursal model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            throw new InvalidOperationException("El identificador de la sucursal no coincide.");
        }

        LimpiarSucursal(model);

        var exists = await _db.Sucursales.AnyAsync(x => x.Codigo == model.Codigo && x.Id != model.Id, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Ya existe otra sucursal con este codigo.");
        }

        _db.Sucursales.Update(model);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task CambiarEstadoAsync(int id, CancellationToken cancellationToken = default)
    {
        var sucursal = await _db.Sucursales.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro la sucursal.");

        sucursal.Activa = !sucursal.Activa;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void LimpiarSucursal(Sucursal model)
    {
        model.Codigo = model.Codigo.Trim();
        model.Nombre = model.Nombre.Trim();
        model.HostFtp = model.HostFtp.Trim();
        model.UsuarioFtp = model.UsuarioFtp.Trim();
        model.PasswordFtp = model.PasswordFtp.Trim();
        model.RutaDestino = model.RutaDestino.Trim();

        if (model.PuertoFtp <= 0)
        {
            model.PuertoFtp = 21;
        }

        if (string.IsNullOrWhiteSpace(model.RutaDestino))
        {
            model.RutaDestino = "/updates";
        }
    }
}
