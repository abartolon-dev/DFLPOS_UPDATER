using DflPosUpdater.App.BLL.Services.Sucursales;
using DflPosUpdater.App.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DflPosUpdater.Web.Controllers;

public class SucursalesController : Controller
{
    private readonly ISucursalService _sucursalService;

    public SucursalesController(ISucursalService sucursalService)
    {
        _sucursalService = sucursalService;
    }

    public async Task<IActionResult> Index()
    {
        var sucursales = await _sucursalService.ListarAsync();
        return View(sucursales);
    }

    public IActionResult Crear()
    {
        return View(new Sucursal { PuertoFtp = 21, RutaDestino = "/updates", Activa = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Sucursal model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _sucursalService.CrearAsync(model);
            TempData["Success"] = "Sucursal creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Editar(int id)
    {
        var sucursal = await _sucursalService.ObtenerAsync(id);
        return sucursal is null ? NotFound() : View(sucursal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Sucursal model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _sucursalService.ActualizarAsync(id, model);
            TempData["Success"] = "Sucursal actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        try
        {
            await _sucursalService.CambiarEstadoAsync(id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
