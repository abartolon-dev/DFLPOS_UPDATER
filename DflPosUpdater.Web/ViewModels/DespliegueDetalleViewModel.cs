using DflPosUpdater.App.Entities;

namespace DflPosUpdater.Web.ViewModels;

public class DespliegueDetalleViewModel
{
    public Despliegue Despliegue { get; set; } = new();
    public List<DespliegueLog> Logs { get; set; } = new();
}
