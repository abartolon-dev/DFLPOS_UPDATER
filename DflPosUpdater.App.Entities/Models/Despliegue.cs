using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.App.Entities;

public class Despliegue
{
    public int Id { get; set; }

    public int VersionAppId { get; set; }
    public VersionApp? VersionApp { get; set; }

    public int SucursalId { get; set; }
    public Sucursal? Sucursal { get; set; }

    public DespliegueEstado Estado { get; set; } = DespliegueEstado.Pendiente;

    public int Intentos { get; set; }

    [StringLength(100)]
    public string? HangfireJobId { get; set; }

    [StringLength(2000)]
    public string? MensajeError { get; set; }

    [StringLength(500)]
    public string? RutaRemotaCarpeta { get; set; }

    // Campo conservado para compatibilidad con la primera version del proyecto.
    [StringLength(500)]
    public string? RutaRemotaZip { get; set; }

    [StringLength(500)]
    public string? RutaRemotaManifest { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public DateTime? FechaNotificacion { get; set; }

    public ICollection<DespliegueLog> Logs { get; set; } = new List<DespliegueLog>();
}
