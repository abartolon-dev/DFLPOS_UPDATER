namespace DflPosUpdater.App.BLL.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardResumenDto> ObtenerResumenAsync(CancellationToken cancellationToken = default);
}
