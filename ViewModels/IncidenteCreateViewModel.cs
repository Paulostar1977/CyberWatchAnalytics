/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para registrar nuevos incidentes de
 *               seguridad asociados a un activo tecnológico.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberWatchAnalytics.ViewModels;

public class IncidenteCreateViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un activo tecnológico.")]
    [Display(Name = "Activo tecnológico")]
    public int IdActivo { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(120, ErrorMessage = "El título no puede superar los 120 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar una criticidad.")]
    public string Criticidad { get; set; } = "Media";

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public string Estado { get; set; } = "Abierto";

    public List<SelectListItem> Activos { get; set; } = new();
}