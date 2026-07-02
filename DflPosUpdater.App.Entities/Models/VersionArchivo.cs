using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.App.Entities;

public class VersionArchivo
{
    public int Id { get; set; }

    public int VersionAppId { get; set; }
    public VersionApp? VersionApp { get; set; }

    [Required, StringLength(255)]
    public string NombreArchivo { get; set; } = string.Empty;

    // Ruta relativa dentro de la carpeta de la version. Ej: dflpos.exe, lib/archivo.dll
    [Required, StringLength(700)]
    public string RutaRelativa { get; set; } = string.Empty;

    // Ruta web local dentro de wwwroot donde se guardo el archivo.
    [Required, StringLength(700)]
    public string RutaWeb { get; set; } = string.Empty;

    public long TamanoBytes { get; set; }

    [Required, StringLength(128)]
    public string Sha256 { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaCarga { get; set; } = DateTime.Now;
}
