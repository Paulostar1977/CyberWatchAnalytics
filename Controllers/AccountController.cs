/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado del inicio y cierre de sesión de los
 *               usuarios del sistema mediante validación segura de claves.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class AccountController : Controller
{
    private readonly CyberWatchDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AccountController(CyberWatchDbContext context)
    {
        _context = context;
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

        var correo = model.Correo.Trim();

        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u =>
                u.Correo == correo &&
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
}