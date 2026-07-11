using System;
using System.Collections.Generic;

namespace CyberWatchAnalytics.Models;

public partial class Reporte
{
    public int IdReporte { get; set; }

    public int IdUsuario { get; set; }

    public string TipoReporte { get; set; } = null!;

    public DateTime FechaGeneracion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
