/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para mostrar los indicadores principales,
 *               incidentes recientes y vulnerabilidades recientes del
 *               panel de control.
 **************************************************************************/

using CyberWatchAnalytics.Models;

namespace CyberWatchAnalytics.ViewModels;

public class DashboardViewModel
{
    public int TotalUsuarios { get; set; }

    public int TotalActivos { get; set; }

    public int TotalIncidentes { get; set; }

    public int TotalVulnerabilidades { get; set; }

    public List<IncidentesSeguridad> UltimosIncidentes { get; set; } = new();

    public List<Vulnerabilidad> UltimasVulnerabilidades { get; set; } = new();
}