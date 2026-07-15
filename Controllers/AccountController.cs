/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado del inicio, cierre de sesión y
 *               recuperación segura de credenciales de los usuarios.
 **************************************************************************/

using System.Security.Cryptography;
using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.Services;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class AccountController : Controller
{
    private readonly CyberWatchDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly EmailService _emailService;

    public AccountController(
        CyberWatchDbContext context,
        EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    //======================================================================
    // FORMULARIO DE INICIO DE SESIÓN
    //======================================================================

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("IdUsuario") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    //======================================================================
    // VALIDACIÓN DE CREDENCIALES
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string correoNormalizado = model.Correo.Trim().ToLower();

        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u =>
                u.Correo.ToLower() == correoNormalizado &&
                u.Estado == "Activo");

        if (usuario == null)
        {
            model.MensajeError = "Correo o contraseña incorrectos.";
            return View(model);
        }

        bool passwordValida = VerificarPassword(
            usuario,
            model.Password,
            out bool requiereMigracion);

        if (!passwordValida)
        {
            model.MensajeError = "Correo o contraseña incorrectos.";
            return View(model);
        }

        /*
         * Los usuarios creados antes de implementar el hash pueden conservar
         * temporalmente una contraseña en texto simple. Después del primer
         * acceso correcto, esta se reemplaza automáticamente por un hash.
         */
        if (requiereMigracion)
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(
                usuario,
                model.Password);

            await _context.SaveChangesAsync();
        }

        HttpContext.Session.SetInt32(
            "IdUsuario",
            usuario.IdUsuario);

        HttpContext.Session.SetString(
            "NombreUsuario",
            usuario.Nombre);

        HttpContext.Session.SetString(
            "RolUsuario",
            usuario.IdRolNavigation.NombreRol);

        return RedirectToAction("Index", "Home");
    }

    //======================================================================
    // FORMULARIO PARA SOLICITAR RECUPERACIÓN DE CONTRASEÑA
    //======================================================================

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    //======================================================================
    // GENERAR Y ENVIAR CÓDIGO DE RECUPERACIÓN
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string correoNormalizado = model.Correo.Trim().ToLower();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Correo.ToLower() == correoNormalizado &&
                u.Estado == "Activo");

        /*
         * Por seguridad, se muestra el mismo mensaje aunque el correo no
         * exista. Esto evita revelar qué direcciones están registradas.
         */
        if (usuario == null)
        {
            model.Mensaje =
                "Si el correo está registrado, recibirá un código de recuperación.";

            return View(model);
        }

        string codigo = GenerarCodigoRecuperacion();

        usuario.CodigoRecuperacion = codigo;
        usuario.FechaExpiracionCodigo = DateTime.Now.AddMinutes(15);

        await _context.SaveChangesAsync();

        try
        {
            await _emailService.EnviarCodigoRecuperacionAsync(
                usuario.Correo,
                usuario.Nombre,
                codigo);
        }
        catch
        {
            /*
             * Si el correo no puede enviarse, se eliminan los datos temporales
             * para evitar que quede un código válido que el usuario no recibió.
             */
            usuario.CodigoRecuperacion = null;
            usuario.FechaExpiracionCodigo = null;

            await _context.SaveChangesAsync();

            ModelState.AddModelError(
                string.Empty,
                "No fue posible enviar el correo de recuperación. Intente nuevamente.");

            return View(model);
        }

        TempData["CorreoRecuperacion"] = usuario.Correo;
        TempData["MensajeExito"] =
            "Se envió un código de recuperación a su correo electrónico.";

        return RedirectToAction(nameof(ResetPassword));
    }

    //======================================================================
    // FORMULARIO PARA RESTABLECER LA CONTRASEÑA
    //======================================================================

    [HttpGet]
    public IActionResult ResetPassword()
    {
        var model = new ResetPasswordViewModel
        {
            Correo = TempData["CorreoRecuperacion"]?.ToString() ?? string.Empty
        };

        return View(model);
    }

    //======================================================================
    // VALIDAR CÓDIGO Y GUARDAR NUEVA CONTRASEÑA
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string correoNormalizado = model.Correo.Trim().ToLower();
        string codigoIngresado = model.Codigo.Trim();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Correo.ToLower() == correoNormalizado &&
                u.Estado == "Activo");

        if (usuario == null)
        {
            model.MensajeError =
                "El código de recuperación no es válido.";

            return View(model);
        }

        bool codigoValido =
            !string.IsNullOrWhiteSpace(usuario.CodigoRecuperacion) &&
            usuario.CodigoRecuperacion == codigoIngresado &&
            usuario.FechaExpiracionCodigo.HasValue &&
            usuario.FechaExpiracionCodigo.Value >= DateTime.Now;

        if (!codigoValido)
        {
            model.MensajeError =
                "El código es incorrecto o ha expirado.";

            return View(model);
        }

        usuario.PasswordHash = _passwordHasher.HashPassword(
            usuario,
            model.NuevaPassword);

        /*
         * El código se elimina después de utilizarlo para impedir que pueda
         * ser reutilizado en una nueva solicitud.
         */
        usuario.CodigoRecuperacion = null;
        usuario.FechaExpiracionCodigo = null;

        await _context.SaveChangesAsync();

        TempData["MensajeLogin"] =
            "La contraseña fue actualizada correctamente. Ya puede iniciar sesión.";

        return RedirectToAction(nameof(Login));
    }

    //======================================================================
    // CIERRE DE SESIÓN
    //======================================================================

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }

    //======================================================================
    // MÉTODOS AUXILIARES
    //======================================================================

    private bool VerificarPassword(
        Usuario usuario,
        string passwordIngresada,
        out bool requiereMigracion)
    {
        requiereMigracion = false;

        try
        {
            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                passwordIngresada);

            if (resultado == PasswordVerificationResult.Success ||
                resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                requiereMigracion =
                    resultado == PasswordVerificationResult.SuccessRehashNeeded;

                return true;
            }
        }
        catch (FormatException)
        {
            /*
             * Si el valor almacenado no tiene formato de hash, corresponde
             * a una contraseña antigua guardada en texto simple.
             */
        }

        if (usuario.PasswordHash == passwordIngresada)
        {
            requiereMigracion = true;
            return true;
        }

        return false;
    }

    private static string GenerarCodigoRecuperacion()
    {
        /*
         * RandomNumberGenerator utiliza un generador criptográficamente
         * seguro, adecuado para códigos temporales de recuperación.
         */
        int numero = RandomNumberGenerator.GetInt32(100000, 1000000);

        return numero.ToString();
    }
}