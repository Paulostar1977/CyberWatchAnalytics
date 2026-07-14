/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo utilizado para presentar indicadores generales
 *               y registros recientes del sistema.
 **************************************************************************/

using CyberWatchAnalytics.Models;

namespace CyberWatchAnalytics.ViewModels;

public class ReporteGeneralViewModel
{
    public int TotalUsuarios { get; set; }

    public int TotalActivos { get; set; }

    public int TotalIncidentes { get; set; }

    public int TotalVulnerabilidades { get; set; }

    public List<IncidentesSeguridad> UltimosIncidentes { get; set; } = new();

    public List<Vulnerabilidad> UltimasVulnerabilidades { get; set; } = new();

    public List<ActivosTecnologico> ActivosRegistrados { get; set; } = new();
}