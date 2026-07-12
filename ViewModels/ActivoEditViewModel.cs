/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para modificar la información de un
 *               activo tecnológico registrado en el sistema.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberWatchAnalytics.ViewModels;

public class ActivoEditViewModel
{
    public int IdActivo { get; set; }

    [Required(ErrorMessage = "El nombre del activo es obligatorio.")]
    [Display(Name = "Nombre")]
    [StringLength(100)]
    public string NombreActivo { get; set; } = string.Empty;

    [Display(Name = "Dirección IP")]
    [StringLength(45)]
    public string? DireccionIp { get; set; }

    [Display(Name = "Ubicación")]
    [StringLength(100)]
    public string? Ubicacion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de activo.")]
    [Display(Name = "Tipo de activo")]
    public int IdTipoActivo { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public string Estado { get; set; } = "Activo";

    public List<SelectListItem> Tipos { get; set; } = new();
}