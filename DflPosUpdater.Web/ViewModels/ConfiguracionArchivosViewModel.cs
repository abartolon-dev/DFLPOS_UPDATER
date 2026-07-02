using DflPosUpdater.App.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DflPosUpdater.Web.ViewModels;

public class ConfiguracionArchivosViewModel
{
    public int VersionAppId { get; set; }
    public string NumeroVersion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public VersionEstado Estado { get; set; }

    [StringLength(300)]
    [Display(Name = "Subcarpeta destino opcional")]
    public string? SubcarpetaBase { get; set; }

    [Display(Name = "Sobrescribir si existe")]
    public bool SobrescribirExistentes { get; set; } = true;

    [Display(Name = "Archivos")]
    public List<IFormFile> Archivos { get; set; } = new();

    public List<VersionArchivo> ArchivosActuales { get; set; } = new();
}
