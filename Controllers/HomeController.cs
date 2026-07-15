/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Controlador principal encargado de mostrar el panel de
 *               inicio y los indicadores generales del sistema.
 **************************************************************************/

using System.Diagnostics;
using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Models;
using CyberWatchAnalytics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Controllers;

public class HomeController : Controller
{
    private readonly CyberWatchDbContext _context;

    public HomeController(CyberWatchDbContext context)
    {
        _context = context;
    }

    //======================================================================
    // PANEL PRINCIPAL DEL SISTEMA
    //======================================================================

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetInt32("IdUsuario") == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var dashboard = new DashboardViewModel
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
                .ToListAsync()
        };

        return View(dashboard);
    }

    //======================================================================
    // MANEJO DE ERRORES
    //======================================================================

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId =
                Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}