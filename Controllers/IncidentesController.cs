/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado de administrar los incidentes de
 *               seguridad registrados en el sistema.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class IncidentesController : Controller
{
    private readonly CyberWatchDbContext _context;

    public IncidentesController(CyberWatchDbContext context)
    {
        _context = context;
    }

    //======================================================================
    // LISTADO DE INCIDENTES DE SEGURIDAD
    //======================================================================

    public async Task<IActionResult> Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var incidentes = await _context.IncidentesSeguridads
            .Include(i => i.IdActivoNavigation)
            .OrderByDescending(i => i.FechaRegistro)
            .ToListAsync();

        return View(incidentes);
    }

    //======================================================================
    // FORMULARIO CREAR INCIDENTE
    //======================================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new IncidenteCreateViewModel
        {
            Activos = await ObtenerActivosAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR NUEVO INCIDENTE
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IncidenteCreateViewModel model)
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

        var incidente = new IncidentesSeguridad
        {
            IdActivo = model.IdActivo,
            Titulo = model.Titulo.Trim(),
            Descripcion = model.Descripcion.Trim(),
            Criticidad = model.Criticidad,
            Estado = model.Estado,
            FechaRegistro = DateTime.Now
        };

        _context.IncidentesSeguridads.Add(incidente);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El incidente de seguridad fue registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // FORMULARIO EDITAR INCIDENTE
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

        var incidente = await _context.IncidentesSeguridads
            .FindAsync(id);

        if (incidente == null)
        {
            return NotFound();
        }

        var model = new IncidenteEditViewModel
        {
            IdIncidente = incidente.IdIncidente,
            IdActivo = incidente.IdActivo,
            Titulo = incidente.Titulo,
            Descripcion = incidente.Descripcion,
            Criticidad = incidente.Criticidad,
            Estado = incidente.Estado,
            Activos = await ObtenerActivosAsync()
        };

        return View(model);
    }

    //======================================================================
    // GUARDAR MODIFICACIÓN DE INCIDENTE
    //======================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(IncidenteEditViewModel model)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var incidente = await _context.IncidentesSeguridads
            .FindAsync(model.IdIncidente);

        if (incidente == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Activos = await ObtenerActivosAsync();
            return View(model);
        }

        incidente.IdActivo = model.IdActivo;
        incidente.Titulo = model.Titulo.Trim();
        incidente.Descripcion = model.Descripcion.Trim();
        incidente.Criticidad = model.Criticidad;
        incidente.Estado = model.Estado;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El incidente de seguridad fue actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    //======================================================================
    // CONFIRMACIÓN DE ELIMINACIÓN DE INCIDENTE
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

        var incidente = await _context.IncidentesSeguridads
            .Include(i => i.IdActivoNavigation)
            .FirstOrDefaultAsync(i => i.IdIncidente == id);

        if (incidente == null)
        {
            return NotFound();
        }

        return View(incidente);
    }

    //======================================================================
    // ELIMINAR INCIDENTE
    //======================================================================

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var incidente = await _context.IncidentesSeguridads
            .FindAsync(id);

        if (incidente == null)
        {
            return NotFound();
        }

        _context.IncidentesSeguridads.Remove(incidente);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            "El incidente de seguridad fue eliminado correctamente.";

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