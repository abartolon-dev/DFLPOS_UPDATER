using DflPosUpdater.App.Entities;

namespace DflPosUpdater.Web.ViewModels;

public class DashboardViewModel
{
    public VersionApp? VersionPublicada { get; set; }
    public int TotalSucursalesActivas { get; set; }
    public int TotalVersiones { get; set; }
    public int DesplieguesPendientes { get; set; }
    public int DesplieguesEnProceso { get; set; }
    public int DesplieguesExitosos { get; set; }
    public int DesplieguesFallidos { get; set; }
    public List<Despliegue> DesplieguesRecientes { get; set; } = new();
}
