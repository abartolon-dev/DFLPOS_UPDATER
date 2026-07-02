using DflPosUpdater.App.BLL.Services.Dashboard;
using DflPosUpdater.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DflPosUpdater.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var resumen = await _dashboardService.ObtenerResumenAsync();

        var model = new DashboardViewModel
        {
            VersionPublicada = resumen.VersionPublicada,
            TotalSucursalesActivas = resumen.TotalSucursalesActivas,
            TotalVersiones = resumen.TotalVersiones,
            DesplieguesPendientes = resumen.DesplieguesPendientes,
            DesplieguesEnProceso = resumen.DesplieguesEnProceso,
            DesplieguesExitosos = resumen.DesplieguesExitosos,
            DesplieguesFallidos = resumen.DesplieguesFallidos,
            DesplieguesRecientes = resumen.DesplieguesRecientes
        };

        return View(model);
    }
}
