/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Servicio encargado de enviar correos electrónicos para
 *               la recuperación de credenciales de los usuarios.
 **************************************************************************/

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace CyberWatchAnalytics.Services;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task EnviarCodigoRecuperacionAsync(
        string correoDestino,
        string nombreUsuario,
        string codigo)
    {
        using var mensaje = new MailMessage
        {
            From = new MailAddress(
                _settings.SenderEmail,
                "CyberWatch Analytics"),

            Subject = "Código de recuperación de contraseña",

            Body = $"""
                    Hola {nombreUsuario},

                    Se solicitó la recuperación de acceso a CyberWatch Analytics.

                    Su código de recuperación es:

                    {codigo}

                    Este código tiene una vigencia de 15 minutos.

                    Si usted no realizó esta solicitud, puede ignorar este mensaje.

                    CyberWatch Analytics
                    """,

            IsBodyHtml = false
        };

        mensaje.To.Add(correoDestino);

        using var smtp = new SmtpClient(
            _settings.SmtpServer,
            _settings.SmtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _settings.SenderEmail,
                _settings.AppPassword)
        };

        await smtp.SendMailAsync(mensaje);
    }
}