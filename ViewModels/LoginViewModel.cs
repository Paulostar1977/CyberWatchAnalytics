/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para capturar las credenciales ingresadas
 *               por el usuario en el inicio de sesión.
 **************************************************************************/

namespace CyberWatchAnalytics.ViewModels;

public class LoginViewModel
{
    public string Correo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? MensajeError { get; set; }
}