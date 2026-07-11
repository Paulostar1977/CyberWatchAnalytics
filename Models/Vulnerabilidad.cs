using System;
using System.Collections.Generic;

namespace CyberWatchAnalytics.Models;

public partial class Vulnerabilidad
{
    public int IdVulnerabilidad { get; set; }

    public int IdActivo { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Severidad { get; set; } = null!;

    public string EstadoMitigacion { get; set; } = null!;

    public DateTime FechaDeteccion { get; set; }

    public virtual ActivosTecnologico IdActivoNavigation { get; set; } = null!;
}
