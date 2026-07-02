using DflPosUpdater.App.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DflPosUpdater.App.BLL.Jobs;

public class CleanupJob
{
    private readonly AppDbContext _db;
    private readonly ILogger<CleanupJob> _logger;

    public CleanupJob(AppDbContext db, ILogger<CleanupJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task EjecutarAsync(int diasRetencion)
    {
        var limite = DateTime.Now.AddDays(-diasRetencion);

        var logs = await _db.DespliegueLogs
            .Where(x => x.Fecha < limite)
            .Take(5000)
            .ToListAsync();

        if (logs.Count == 0)
        {
            return;
        }

        _db.DespliegueLogs.RemoveRange(logs);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Se eliminaron {Count} logs antiguos de despliegue.", logs.Count);
    }
}
