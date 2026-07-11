using System;
using System.Collections.Generic;

namespace CyberWatchAnalytics.Models;

public partial class HistorialEvento
{
    public int IdEvento { get; set; }

    public int IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public DateTime FechaEvento { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
