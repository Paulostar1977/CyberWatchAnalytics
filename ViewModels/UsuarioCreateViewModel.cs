/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para registrar nuevos usuarios en el
 *               sistema mediante el formulario de creación.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberWatchAnalytics.ViewModels;

public class UsuarioCreateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    [Display(Name = "Rol")]
    public int IdRol { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public string Estado { get; set; } = "Activo";

    public IEnumerable<SelectListItem> Roles { get; set; } =
        Enumerable.Empty<SelectListItem>();
}