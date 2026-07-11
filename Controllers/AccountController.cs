/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado del inicio y cierre de sesión de los
 *               usuarios del sistema.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class AccountController : Controller
{
    private readonly CyberWatchDbContext _context;

    public AccountController(CyberWatchDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("IdUsuario") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefault(u =>
                u.Correo == model.Correo &&
                u.PasswordHash == model.Password &&
                u.Estado == "Activo");

        if (usuario == null)
        {
            model.MensajeError = "Correo o contraseña incorrectos.";
            return View(model);
        }

        HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
        HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
        HttpContext.Session.SetString("RolUsuario", usuario.IdRolNavigation.NombreRol);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }
}