using DflPosUpdater.App.Entities;

namespace DflPosUpdater.App.BLL.Services.Notifications;

public interface INotificationService
{
    string CrearManifestJson(VersionApp version, IEnumerable<VersionArchivo> archivos, string rutaRemotaCarpeta);
}
