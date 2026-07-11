using System;
using System.Collections.Generic;

namespace CyberWatchAnalytics.Models;

public partial class ActivosTecnologico
{
    public int IdActivo { get; set; }

    public int IdTipoActivo { get; set; }

    public string NombreActivo { get; set; } = null!;

    public string? DireccionIp { get; set; }

    public string? Ubicacion { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual TiposActivo IdTipoActivoNavigation { get; set; } = null!;

    public virtual ICollection<IncidentesSeguridad> IncidentesSeguridads { get; set; } = new List<IncidentesSeguridad>();

    public virtual ICollection<Vulnerabilidad> Vulnerabilidades { get; set; } = new List<Vulnerabilidad>();
}
