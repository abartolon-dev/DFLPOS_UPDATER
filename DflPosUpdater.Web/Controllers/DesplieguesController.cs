using DflPosUpdater.App.BLL.Services.Deploy;
using DflPosUpdater.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DflPosUpdater.Web.Controllers;

public class DesplieguesController : Controller
{
    private readonly IDeployService _deployService;

    public DesplieguesController(IDeployService deployService)
    {
        _deployService = deployService;
    }

    public async Task<IActionResult> Index(int? versionId = null)
    {
        var despliegues = await _deployService.ListarDesplieguesAsync(versionId);
        return View(despliegues);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var despliegue = await _deployService.ObtenerDespliegueAsync(id);
        if (despliegue is null)
        {
            return NotFound();
        }

        var logs = await _deployService.ObtenerLogsAsync(id);

        return View(new DespliegueDetalleViewModel
        {
            Despliegue = despliegue,
            Logs = logs
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reintentar(int id)
    {
        try
        {
            await _deployService.ReintentarDespliegueAsync(id);
            TempData["Success"] = "Reintento encolado correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await _deployService.CancelarDespliegueAsync(id);
            TempData["Success"] = "Despliegue cancelado.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalle), new { id });
    }
}
