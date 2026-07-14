/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador encargado de generar los reportes generales
 *               del sistema.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class ReportesController : Controller
{
    private readonly CyberWatchDbContext _context;

    public ReportesController(CyberWatchDbContext context)
    {
        _context = context;
    }

    //======================================================================
    // REPORTE GENERAL DEL SISTEMA
    //======================================================================

    public async Task<IActionResult> Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ReporteGeneralViewModel
        {
            TotalUsuarios = await _context.Usuarios.CountAsync(),

            TotalActivos = await _context.ActivosTecnologicos.CountAsync(),

            TotalIncidentes = await _context.IncidentesSeguridads.CountAsync(),

            TotalVulnerabilidades = await _context.Vulnerabilidades.CountAsync(),

            UltimosIncidentes = await _context.IncidentesSeguridads
                .Include(i => i.IdActivoNavigation)
                .OrderByDescending(i => i.FechaRegistro)
                .Take(5)
                .ToListAsync(),

            UltimasVulnerabilidades = await _context.Vulnerabilidades
                .Include(v => v.IdActivoNavigation)
                .OrderByDescending(v => v.FechaDeteccion)
                .Take(5)
                .ToListAsync(),

            ActivosRegistrados = await _context.ActivosTecnologicos
                .Include(a => a.IdTipoActivoNavigation)
                .OrderBy(a => a.NombreActivo)
                .Take(10)
                .ToListAsync()
        };

        return View(model);
    }

    //======================================================================
    // MÉTODOS AUXILIARES
    //======================================================================

    private bool UsuarioAutenticado()
    {
        return HttpContext.Session.GetInt32("IdUsuario") != null;
    }
}