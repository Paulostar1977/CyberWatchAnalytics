/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para modificar una vulnerabilidad
 *               registrada en el sistema.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberWatchAnalytics.ViewModels;

public class VulnerabilidadEditViewModel
{
    public int IdVulnerabilidad { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un activo tecnológico.")]
    [Display(Name = "Activo tecnológico")]
    public int IdActivo { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar una severidad.")]
    public string Severidad { get; set; } = "Media";

    [Required(ErrorMessage = "Debe seleccionar un estado de mitigación.")]
    [Display(Name = "Estado de mitigación")]
    public string EstadoMitigacion { get; set; } = "Pendiente";

    public List<SelectListItem> Activos { get; set; } = new();
}