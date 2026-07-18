/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Configuración utilizada para el envío de correos
 *               electrónicos mediante el servidor SMTP de Gmail.
 **************************************************************************/

namespace CyberWatchAnalytics.Services;

public class EmailSettings
{
    public string SenderEmail { get; set; } = string.Empty;

    public string AppPassword { get; set; } = string.Empty;

    public string SmtpServer { get; set; } = "smtp.gmail.com";

    public int SmtpPort { get; set; } = 587;

    /*
     * Correo utilizado para pruebas.
     * Si se configura, todos los códigos de recuperación
     * serán enviados a esta dirección.
     * Si queda vacío, se enviarán al correo real del usuario.
     */
    public string? RecoveryTestEmail { get; set; }
}