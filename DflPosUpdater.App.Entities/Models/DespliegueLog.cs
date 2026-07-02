using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.App.Entities;

public class DespliegueLog
{
    public int Id { get; set; }

    public int DespliegueId { get; set; }
    public Despliegue? Despliegue { get; set; }

    public LogTipo Tipo { get; set; } = LogTipo.Info;

    [Required, StringLength(1000)]
    public string Mensaje { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Detalle { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
