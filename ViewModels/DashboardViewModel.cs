/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para mostrar los indicadores principales
 *               del panel de control (Dashboard).
 **************************************************************************/

namespace CyberWatchAnalytics.ViewModels;

public class DashboardViewModel
{
    public int TotalUsuarios { get; set; }

    public int TotalActivos { get; set; }

    public int TotalIncidentes { get; set; }

    public int TotalVulnerabilidades { get; set; }
}