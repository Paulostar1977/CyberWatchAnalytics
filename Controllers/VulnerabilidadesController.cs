/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado de administrar las vulnerabilidades
 *               registradas en el sistema.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class VulnerabilidadesController : Controller
{
    private readonly CyberWatchDbContext _context;

    public VulnerabilidadesController(CyberWatchDbContext context)
    {
        _context = context;
    }

    //======================================================================
    // LISTADO DE VULNERABILIDADES
    //======================================================================

    public async Task<IActionResult> Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var vulnerabilidades = await _context.Vulnerabilidades
            .Include(v => v.IdActivoNavigation)
            .OrderByDescending(v => v.FechaDeteccion)
            .ToListAsync();

        return View(vulnerabilidades);
    }

    //======================================================================
    // FORMULARIO CREAR VULNERABILIDAD
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new VulnerabilidadCreateViewModel
        {
            Activos = await ObtenerActivosAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR NUEVA VULNERABILIDAD
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VulnerabilidadCreateViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            model.Activos = await ObtenerActivosAsync();
            return View(model);
        }

        var vulnerabilidad = new Vulnerabilidad
        {
            IdActivo = model.IdActivo,
            Descripcion = model.Descripcion.Trim(),
            Severidad = model.Severidad,
            EstadoMitigacion = model.EstadoMitigacion,
            FechaDeteccion = DateTime.Now
        };

        _context.Vulnerabilidades.Add(vulnerabilidad);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La vulnerabilidad fue registrada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // FORMULARIO EDITAR VULNERABILIDAD
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

        var vulnerabilidad = await _context.Vulnerabilidades
            .FindAsync(id);

        if (vulnerabilidad == null)
        {
            return NotFound();
        }

        var model = new VulnerabilidadEditViewModel
        {
            IdVulnerabilidad = vulnerabilidad.IdVulnerabilidad,
            IdActivo = vulnerabilidad.IdActivo,
            Descripcion = vulnerabilidad.Descripcion,
            Severidad = vulnerabilidad.Severidad,
            EstadoMitigacion = vulnerabilidad.EstadoMitigacion,
            Activos = await ObtenerActivosAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR MODIFICACIÓN DE VULNERABILIDAD
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VulnerabilidadEditViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var vulnerabilidad = await _context.Vulnerabilidades
            .FindAsync(model.IdVulnerabilidad);

        if (vulnerabilidad == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Activos = await ObtenerActivosAsync();
            return View(model);
        }

        vulnerabilidad.IdActivo = model.IdActivo;
        vulnerabilidad.Descripcion = model.Descripcion.Trim();
        vulnerabilidad.Severidad = model.Severidad;
        vulnerabilidad.EstadoMitigacion = model.EstadoMitigacion;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La vulnerabilidad fue actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // CONFIRMACIÓN DE ELIMINACIÓN DE VULNERABILIDAD
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

        var vulnerabilidad = await _context.Vulnerabilidades
            .Include(v => v.IdActivoNavigation)
            .FirstOrDefaultAsync(v => v.IdVulnerabilidad == id);

        if (vulnerabilidad == null)
        {
            return NotFound();
        }

        return View(vulnerabilidad);
    }

    //======================================================================
    // ELIMINAR VULNERABILIDAD
    //======================================================================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var vulnerabilidad = await _context.Vulnerabilidades
            .FindAsync(id);

        if (vulnerabilidad == null)
        {
            return NotFound();
        }

        _context.Vulnerabilidades.Remove(vulnerabilidad);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "La vulnerabilidad fue eliminada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // MÉTODOS AUXILIARES
    //======================================================================

    private bool UsuarioAutenticado()
    {
        return HttpContext.Session.GetInt32("IdUsuario") != null;
    }

    private async Task<List<SelectListItem>> ObtenerActivosAsync()
    {
        return await _context.ActivosTecnologicos
            .OrderBy(a => a.NombreActivo)
            .Select(a => new SelectListItem
            {
                Value = a.IdActivo.ToString(),
                Text = a.NombreActivo
            })
            .ToListAsync();
    }
}