using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.App.Entities;

public class VersionApp
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    [Display(Name = "Version")]
    public string NumeroVersion { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    // Nuevo flujo: se guarda una carpeta por version, no un ZIP.
    [Required, StringLength(500)]
    public string RutaCarpeta { get; set; } = string.Empty;

    // Campos conservados para compatibilidad con la primera version del proyecto.
    // En el flujo nuevo RutaZip guarda la misma ruta de RutaCarpeta.
    [Required, StringLength(500)]
    public string RutaZip { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string NombreArchivoOriginal { get; set; } = string.Empty;

    public long TamanoBytes { get; set; }

    public int TotalArchivos { get; set; }

    [Required, StringLength(128)]
    public string Sha256 { get; set; } = string.Empty;

    public VersionEstado Estado { get; set; } = VersionEstado.Borrador;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaPublicacion { get; set; }

    public ICollection<VersionArchivo> Archivos { get; set; } = new List<VersionArchivo>();

    public ICollection<Despliegue> Despliegues { get; set; } = new List<Despliegue>();
}
