using DflPosUpdater.App.BLL.Services.Deploy;
using DflPosUpdater.App.BLL.Services.Sucursales;
using DflPosUpdater.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DflPosUpdater.Web.Controllers;

public class VersionesController : Controller
{
    private readonly IVersionService _versionService;
    private readonly IDeployService _deployService;
    private readonly ISucursalService _sucursalService;

    public VersionesController(
        IVersionService versionService,
        IDeployService deployService,
        ISucursalService sucursalService)
    {
        _versionService = versionService;
        _deployService = deployService;
        _sucursalService = sucursalService;
    }

    public async Task<IActionResult> Index()
    {
        var versiones = await _versionService.ListarVersionesAsync();
        return View(versiones);
    }

    public async Task<IActionResult> Crear()
    {
        return View(await CrearViewModelAsync(new CrearVersionViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(524_288_000)]
    public async Task<IActionResult> Crear(CrearVersionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await CrearViewModelAsync(model));
        }

        try
        {
            var version = await _versionService.CrearVersionAsync(new CrearVersionRequest
            {
                NumeroVersion = model.NumeroVersion,
                Descripcion = model.Descripcion,
                Archivos = model.Archivos,
                SubcarpetaBase = model.SubcarpetaBase
            });

            if (model.PublicarAlCrear)
            {
                var ids = model.SucursalesSeleccionadas.Length > 0 ? model.SucursalesSeleccionadas : null;
                await _deployService.PublicarVersionAsync(version.Id, ids);
            }

            TempData["Success"] = "Version creada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = version.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await CrearViewModelAsync(model));
        }
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var version = await _versionService.ObtenerVersionAsync(id);
        if (version is null)
        {
            return NotFound();
        }

        var despliegues = await _versionService.ObtenerDesplieguesPorVersionAsync(id);

        return View(new VersionDetalleViewModel
        {
            Version = version,
            Despliegues = despliegues
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publicar(int id)
    {
        try
        {
            await _deployService.PublicarVersionAsync(id);
            TempData["Success"] = "Publicacion encolada para sucursales activas.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalle), new { id });
    }

    private async Task<CrearVersionViewModel> CrearViewModelAsync(CrearVersionViewModel model)
    {
        var sucursales = await _sucursalService.ListarActivasAsync();
        model.SucursalesDisponibles = sucursales
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Nombre,
                Selected = model.SucursalesSeleccionadas.Contains(x.Id)
            })
            .ToList();

        return model;
    }
}
