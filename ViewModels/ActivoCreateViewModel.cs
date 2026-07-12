using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CyberWatchAnalytics.ViewModels;

public class ActivoCreateViewModel
{
    [Required]
    [Display(Name = "Nombre")]
    public string NombreActivo { get; set; } = string.Empty;

    [Display(Name = "Dirección IP")]
    public string? DireccionIp { get; set; }

    [Display(Name = "Ubicación")]
    public string? Ubicacion { get; set; }

    [Required]
    [Display(Name = "Tipo de activo")]
    public int IdTipoActivo { get; set; }

    [Required]
    public string Estado { get; set; } = "Activo";

    public List<SelectListItem> Tipos { get; set; } = new();
}