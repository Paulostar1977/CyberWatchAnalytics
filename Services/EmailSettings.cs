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
}