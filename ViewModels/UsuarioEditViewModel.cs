/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para modificar la información de un
 *               usuario registrado en el sistema.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberWatchAnalytics.ViewModels;

public class UsuarioEditViewModel
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Display(Name = "Nueva contraseña")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    public int IdRol { get; set; }

    [Required]
    public string Estado { get; set; } = "Activo";

    public IEnumerable<SelectListItem> Roles { get; set; } =
        Enumerable.Empty<SelectListItem>();
}