/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado de administrar los usuarios del
 *               sistema mediante operaciones de consulta, creación,
 *               modificación y eliminación.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class UsuariosController : Controller
{
    private readonly CyberWatchDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public UsuariosController(CyberWatchDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    //======================================================================
    // LISTADO DE USUARIOS
    //======================================================================

    public async Task<IActionResult> Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            TempData["MensajeError"] =
                "No tiene permisos para acceder a la administración de usuarios.";

            return RedirectToAction("Index", "Home");
        }

        var usuarios = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .OrderBy(u => u.Nombre)
            .ToListAsync();

        return View(usuarios);
    }

    //======================================================================
    // FORMULARIO CREAR USUARIO
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new UsuarioCreateViewModel
        {
            Roles = await ObtenerRolesAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR NUEVO USUARIO
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            return RedirectToAction("Index", "Home");
        }

        string correoNormalizado = model.Correo.Trim().ToLower();

        bool correoExiste = await _context.Usuarios
            .AnyAsync(u => u.Correo.ToLower() == correoNormalizado);

        if (correoExiste)
        {
            ModelState.AddModelError(
                nameof(model.Correo),
                "Ya existe un usuario registrado con ese correo.");
        }

        if (!ModelState.IsValid)
        {
            model.Roles = await ObtenerRolesAsync();
            return View(model);
        }

        var usuario = new Usuario
        {
            Nombre = model.Nombre.Trim(),
            Correo = correoNormalizado,
            IdRol = model.IdRol,
            Estado = model.Estado,
            FechaCreacion = DateTime.Now,
            PasswordHash = string.Empty
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(
            usuario,
            model.Password);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El usuario fue registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // FORMULARIO EDITAR USUARIO
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        var model = new UsuarioEditViewModel
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            IdRol = usuario.IdRol,
            Estado = usuario.Estado,
            Roles = await ObtenerRolesAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR MODIFICACIÓN DE USUARIO
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioEditViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            return RedirectToAction("Index", "Home");
        }

        var usuario = await _context.Usuarios.FindAsync(model.IdUsuario);

        if (usuario == null)
        {
            return NotFound();
        }

        string correoNormalizado = model.Correo.Trim().ToLower();

        bool correoDuplicado = await _context.Usuarios.AnyAsync(u =>
            u.Correo.ToLower() == correoNormalizado &&
            u.IdUsuario != model.IdUsuario);

        if (correoDuplicado)
        {
            ModelState.AddModelError(
                nameof(model.Correo),
                "Ya existe otro usuario registrado con ese correo.");
        }

        if (!ModelState.IsValid)
        {
            model.Roles = await ObtenerRolesAsync();
            return View(model);
        }

        usuario.Nombre = model.Nombre.Trim();
        usuario.Correo = correoNormalizado;
        usuario.IdRol = model.IdRol;
        usuario.Estado = model.Estado;

        /*
         * La contraseña solamente se modifica cuando el administrador
         * escribe una nueva. Si el campo queda vacío, se conserva el hash
         * almacenado actualmente.
         */
        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(
                usuario,
                model.Password);
        }

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El usuario fue actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // CONFIRMACIÓN DE ELIMINACIÓN DE USUARIO
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            return RedirectToAction("Index", "Home");
        }

        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    //======================================================================
    // ELIMINAR USUARIO
    //======================================================================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!EsAdministrador())
        {
            return RedirectToAction("Index", "Home");
        }

        int? idUsuarioSesion =
            HttpContext.Session.GetInt32("IdUsuario");

        if (idUsuarioSesion == id)
        {
            TempData["MensajeError"] =
                "No es posible eliminar el usuario que mantiene la sesión activa.";

            return RedirectToAction(nameof(Index));
        }

        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound();
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El usuario fue eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // MÉTODOS AUXILIARES
    //======================================================================

    private bool UsuarioAutenticado()
    {
        return HttpContext.Session.GetInt32("IdUsuario") != null;
    }

    private bool EsAdministrador()
    {
        return HttpContext.Session.GetString("RolUsuario") == "Administrador";
    }

    private async Task<List<SelectListItem>> ObtenerRolesAsync()
    {
        return await _context.Roles
            .OrderBy(r => r.NombreRol)
            .Select(r => new SelectListItem
            {
                Value = r.IdRol.ToString(),
                Text = r.NombreRol
            })
            .ToListAsync();
    }
}