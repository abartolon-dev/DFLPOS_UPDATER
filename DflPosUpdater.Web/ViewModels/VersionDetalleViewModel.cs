using DflPosUpdater.App.Entities;

namespace DflPosUpdater.Web.ViewModels;

public class VersionDetalleViewModel
{
    public VersionApp Version { get; set; } = new();
    public List<Despliegue> Despliegues { get; set; } = new();
}
