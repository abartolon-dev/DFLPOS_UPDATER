using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.App.Entities;

public class Sucursal
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(255)]
    [Display(Name = "Host FTP")]
    public string HostFtp { get; set; } = string.Empty;

    [Display(Name = "Puerto FTP")]
    public int PuertoFtp { get; set; } = 21;

    [Required, StringLength(150)]
    [Display(Name = "Usuario FTP")]
    public string UsuarioFtp { get; set; } = string.Empty;

    [Required, StringLength(255)]
    [DataType(DataType.Password)]
    [Display(Name = "Password FTP")]
    public string PasswordFtp { get; set; } = string.Empty;

    [Required, StringLength(500)]
    [Display(Name = "Ruta destino")]
    public string RutaDestino { get; set; } = "/updates";

    [Display(Name = "Usar FTPS")]
    public bool UsarFtps { get; set; }

    public bool Activa { get; set; } = true;

    [StringLength(50)]
    public string? UltimaVersionInstalada { get; set; }

    public DateTime? UltimaConexion { get; set; }

    public ICollection<Despliegue> Despliegues { get; set; } = new List<Despliegue>();
}
