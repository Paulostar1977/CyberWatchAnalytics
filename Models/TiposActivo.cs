using System;
using System.Collections.Generic;

namespace CyberWatchAnalytics.Models;

public partial class TiposActivo
{
    public int IdTipoActivo { get; set; }

    public string NombreTipo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<ActivosTecnologico> ActivosTecnologicos { get; set; } = new List<ActivosTecnologico>();
}
