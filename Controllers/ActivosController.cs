/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado de administrar los activos
 *               tecnológicos registrados en el sistema.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class ActivosController : Controller
{
    private readonly CyberWatchDbContext _context;

    public ActivosController(CyberWatchDbContext context)
    {
        _context = context;
    }

    //======================================================================
    // LISTADO DE ACTIVOS TECNOLÓGICOS
    //======================================================================

    public async Task<IActionResult> Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var activos = await _context.ActivosTecnologicos
            .Include(a => a.IdTipoActivoNavigation)
            .OrderBy(a => a.NombreActivo)
            .ToListAsync();

        return View(activos);
    }

    //======================================================================
    // FORMULARIO CREAR ACTIVO TECNOLÓGICO
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ActivoCreateViewModel
        {
            Tipos = await ObtenerTiposActivoAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR NUEVO ACTIVO TECNOLÓGICO
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivoCreateViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            model.Tipos = await ObtenerTiposActivoAsync();
            return View(model);
        }

        var activo = new ActivosTecnologico
        {
            NombreActivo = model.NombreActivo.Trim(),
            DireccionIp = LimpiarTextoOpcional(model.DireccionIp),
            Ubicacion = LimpiarTextoOpcional(model.Ubicacion),
            IdTipoActivo = model.IdTipoActivo,
            Estado = model.Estado,
            FechaRegistro = DateTime.Now
        };

        _context.ActivosTecnologicos.Add(activo);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El activo tecnológico fue registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // FORMULARIO EDITAR ACTIVO TECNOLÓGICO
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id == null)
        {
            return NotFound();
        }

        var activo = await _context.ActivosTecnologicos.FindAsync(id);

        if (activo == null)
        {
            return NotFound();
        }

        var model = new ActivoEditViewModel
        {
            IdActivo = activo.IdActivo,
            NombreActivo = activo.NombreActivo,
            DireccionIp = activo.DireccionIp,
            Ubicacion = activo.Ubicacion,
            IdTipoActivo = activo.IdTipoActivo,
            Estado = activo.Estado,
            Tipos = await ObtenerTiposActivoAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR MODIFICACIÓN DE ACTIVO TECNOLÓGICO
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ActivoEditViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var activo = await _context.ActivosTecnologicos
            .FindAsync(model.IdActivo);

        if (activo == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Tipos = await ObtenerTiposActivoAsync();
            return View(model);
        }

        activo.NombreActivo = model.NombreActivo.Trim();
        activo.DireccionIp = LimpiarTextoOpcional(model.DireccionIp);
        activo.Ubicacion = LimpiarTextoOpcional(model.Ubicacion);
        activo.IdTipoActivo = model.IdTipoActivo;
        activo.Estado = model.Estado;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El activo tecnológico fue actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // CONFIRMACIÓN DE ELIMINACIÓN DE ACTIVO TECNOLÓGICO
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id == null)
        {
            return NotFound();
        }

        var activo = await _context.ActivosTecnologicos
            .Include(a => a.IdTipoActivoNavigation)
            .FirstOrDefaultAsync(a => a.IdActivo == id);

        if (activo == null)
        {
            return NotFound();
        }

        return View(activo);
    }

    //======================================================================
    // ELIMINAR ACTIVO TECNOLÓGICO
    //======================================================================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var activo = await _context.ActivosTecnologicos.FindAsync(id);

        if (activo == null)
        {
            return NotFound();
        }

        bool tieneIncidentes = await _context.IncidentesSeguridads
            .AnyAsync(i => i.IdActivo == id);

        bool tieneVulnerabilidades = await _context.Vulnerabilidades
            .AnyAsync(v => v.IdActivo == id);

        if (tieneIncidentes || tieneVulnerabilidades)
        {
            TempData["MensajeError"] =
                "No es posible eliminar el activo porque posee incidentes o vulnerabilidades asociados.";

            return RedirectToAction(nameof(Index));
        }

        _context.ActivosTecnologicos.Remove(activo);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El activo tecnológico fue eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // MÉTODOS AUXILIARES
    //======================================================================

    private bool UsuarioAutenticado()
    {
        return HttpContext.Session.GetInt32("IdUsuario") != null;
    }

    private async Task<List<SelectListItem>> ObtenerTiposActivoAsync()
    {
        return await _context.TiposActivos
            .OrderBy(t => t.NombreTipo)
            .Select(t => new SelectListItem
            {
                Value = t.IdTipoActivo.ToString(),
                Text = t.NombreTipo
            })
            .ToListAsync();
    }

    private static string? LimpiarTextoOpcional(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }
}