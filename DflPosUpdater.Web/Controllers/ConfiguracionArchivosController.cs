using DflPosUpdater.App.BLL.Services.Deploy;
using DflPosUpdater.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DflPosUpdater.Web.Controllers;

public class ConfiguracionArchivosController : Controller
{
    private readonly IVersionService _versionService;

    public ConfiguracionArchivosController(IVersionService versionService)
    {
        _versionService = versionService;
    }

    public async Task<IActionResult> Index()
    {
        var versiones = await _versionService.ListarVersionesAsync();
        return View(versiones);
    }

    public async Task<IActionResult> Editar(int id)
    {
        var version = await _versionService.ObtenerVersionAsync(id);
        if (version is null)
        {
            return NotFound();
        }

        return View(new ConfiguracionArchivosViewModel
        {
            VersionAppId = version.Id,
            NumeroVersion = version.NumeroVersion,
            Descripcion = version.Descripcion,
            Estado = version.Estado,
            SobrescribirExistentes = true,
            ArchivosActuales = version.Archivos.OrderBy(x => x.RutaRelativa).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(524_288_000)]
    public async Task<IActionResult> Editar(ConfiguracionArchivosViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await RellenarDatosVersionAsync(model);
            return View(model);
        }

        try
        {
            await _versionService.AgregarArchivosAsync(
                model.VersionAppId,
                model.Archivos,
                model.SubcarpetaBase,
                model.SobrescribirExistentes);

            TempData["Success"] = "Archivos guardados correctamente.";
            return RedirectToAction(nameof(Editar), new { id = model.VersionAppId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await RellenarDatosVersionAsync(model);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var versionId = await _versionService.EliminarArchivoAsync(id);
            TempData["Success"] = "Archivo eliminado correctamente.";
            return RedirectToAction(nameof(Editar), new { id = versionId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task RellenarDatosVersionAsync(ConfiguracionArchivosViewModel model)
    {
        var version = await _versionService.ObtenerVersionAsync(model.VersionAppId);
        if (version is null)
        {
            return;
        }

        model.NumeroVersion = version.NumeroVersion;
        model.Descripcion = version.Descripcion;
        model.Estado = version.Estado;
        model.ArchivosActuales = version.Archivos.OrderBy(x => x.RutaRelativa).ToList();
    }
}
