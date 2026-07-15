/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para solicitar la recuperación de acceso
 *               mediante el correo electrónico registrado por el usuario.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;

namespace CyberWatchAnalytics.ViewModels;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    public string? Mensaje { get; set; }
}