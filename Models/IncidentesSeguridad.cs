using System;
using System.Collections.Generic;

namespace CyberWatchAnalytics.Models;

public partial class IncidentesSeguridad
{
    public int IdIncidente { get; set; }

    public int IdActivo { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Criticidad { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual ActivosTecnologico IdActivoNavigation { get; set; } = null!;
}
