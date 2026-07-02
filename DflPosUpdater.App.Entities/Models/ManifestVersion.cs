namespace DflPosUpdater.App.Entities;

public class ManifestVersion
{
    public int VersionAppId { get; set; }
    public string NumeroVersion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string CarpetaVersion { get; set; } = string.Empty;
    public string RutaRemotaCarpeta { get; set; } = string.Empty;
    public int TotalArchivos { get; set; }
    public long TamanoBytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public string FechaPublicacion { get; set; } = string.Empty;
    public List<ManifestArchivoVersion> Archivos { get; set; } = new();

    // Compatibilidad con el manifest anterior basado en ZIP.
    public string? Archivo { get; set; }
    public string? RutaRemotaZip { get; set; }
}

public class ManifestArchivoVersion
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaRelativa { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}
