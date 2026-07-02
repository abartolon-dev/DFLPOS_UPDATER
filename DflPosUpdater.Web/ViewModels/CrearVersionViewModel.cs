using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.Web.ViewModels;

public class CrearVersionViewModel
{
    [Required, StringLength(50)]
    [Display(Name = "Numero de version")]
    public string NumeroVersion { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Display(Name = "Archivos de la version")]
    public List<IFormFile> Archivos { get; set; } = new();

    [StringLength(300)]
    [Display(Name = "Subcarpeta destino opcional")]
    public string? SubcarpetaBase { get; set; }

    [Display(Name = "Publicar al crear")]
    public bool PublicarAlCrear { get; set; } = true;

    [Display(Name = "Sucursales")]
    public int[] SucursalesSeleccionadas { get; set; } = Array.Empty<int>();

    public List<SelectListItem> SucursalesDisponibles { get; set; } = new();
}
