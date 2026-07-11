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

namespace CyberWatchAnalytics.Controllers;

public class HomeController : Controller
{
    private readonly CyberWatchDbContext _context;

    public HomeController(CyberWatchDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("IdUsuario") == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var dashboard = new DashboardViewModel
        {
            TotalUsuarios = _context.Usuarios.Count(),
            TotalActivos = _context.ActivosTecnologicos.Count(),
            TotalIncidentes = _context.IncidentesSeguridads.Count(),
            TotalVulnerabilidades = _context.Vulnerabilidades.Count()
        };

        return View(dashboard);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}