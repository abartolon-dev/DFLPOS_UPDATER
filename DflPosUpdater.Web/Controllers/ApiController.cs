using DflPosUpdater.App.BLL.Services.Updater;
using Microsoft.AspNetCore.Mvc;

namespace DflPosUpdater.Web.Controllers;

[ApiController]
[Route("api/updater")]
public class ApiController : ControllerBase
{
    private readonly IUpdaterManifestService _updaterManifestService;

    public ApiController(IUpdaterManifestService updaterManifestService)
    {
        _updaterManifestService = updaterManifestService;
    }

    [HttpGet("manifest/{codigoSucursal}")]
    public async Task<IActionResult> Manifest(string codigoSucursal, [FromQuery] string? versionActual = null)
    {
        var manifest = await _updaterManifestService.ObtenerManifestAsync(codigoSucursal, versionActual);
        if (!manifest.SucursalEncontrada)
        {
            return NotFound(new { message = manifest.Message });
        }

        return Ok(manifest);
    }
}
