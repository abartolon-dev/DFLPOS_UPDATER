using DflPosUpdater.App.DAL;
using DflPosUpdater.App.Entities;
using DflPosUpdater.App.BLL.Services.Deploy;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.BLL.Jobs;

public class RetryFailedDeploymentsJob
{
    private readonly AppDbContext _db;
    private readonly IDeployService _deployService;

    public RetryFailedDeploymentsJob(AppDbContext db, IDeployService deployService)
    {
        _db = db;
        _deployService = deployService;
    }

    public async Task EjecutarAsync()
    {
        var failed = await _db.Despliegues
            .Where(x => x.Estado == DespliegueEstado.Fallido && x.Intentos < 5)
            .OrderBy(x => x.FechaFin)
            .Take(20)
            .ToListAsync();

        foreach (var despliegue in failed)
        {
            await _deployService.ReintentarDespliegueAsync(despliegue.Id);
        }
    }
}
