using System.Text.Json.Serialization;

namespace DflPosUpdater.App.BLL.Services.Updater;

public class UpdaterManifestResponse
{
    [JsonIgnore]
    public bool SucursalEncontrada { get; set; } = true;
    public string? Message { get; set; }
    public bool UpdateAvailable { get; set; }
    public string? Version { get; set; }
    public string? Descripcion { get; set; }
    public string? Sha256 { get; set; }
    public long TamanoBytes { get; set; }
    public int TotalArchivos { get; set; }
    public string? CarpetaVersion { get; set; }
    public string? RutaRemotaCarpeta { get; set; }
    public string? RutaRemotaManifest { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public List<UpdaterManifestArchivoResponse> Archivos { get; set; } = new();

    // Compatibilidad con clientes antiguos basados en ZIP.
    public string? Archivo { get; set; }
    public string? RutaRemotaZip { get; set; }
}

public class UpdaterManifestArchivoResponse
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaRelativa { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}
