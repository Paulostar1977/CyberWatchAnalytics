/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para validar el código de recuperación
 *               y registrar una nueva contraseña para el usuario.
 **************************************************************************/

using System.ComponentModel.DataAnnotations;

namespace CyberWatchAnalytics.ViewModels;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código de recuperación es obligatorio.")]
    [StringLength(
        6,
        MinimumLength = 6,
        ErrorMessage = "El código debe contener 6 caracteres.")]
    [Display(Name = "Código de recuperación")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(NuevaPassword),
        ErrorMessage = "Las contraseñas ingresadas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;

    public string? MensajeError { get; set; }
}