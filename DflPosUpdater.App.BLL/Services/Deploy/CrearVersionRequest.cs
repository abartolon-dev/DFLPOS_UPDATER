using Microsoft.AspNetCore.Http;

namespace DflPosUpdater.App.BLL.Services.Deploy;

public class CrearVersionRequest
{
    public string NumeroVersion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public IEnumerable<IFormFile> Archivos { get; set; } = Enumerable.Empty<IFormFile>();
    public string? SubcarpetaBase { get; set; }
}
