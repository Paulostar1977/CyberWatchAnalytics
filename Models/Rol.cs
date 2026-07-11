/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo que representa los roles disponibles dentro del
 *               sistema y su relación con los usuarios.
 **************************************************************************/

namespace CyberWatchAnalytics.Models;

public partial class Rol
{
    // Clave primaria
    public int IdRol { get; set; }

    // Nombre del rol
    public string NombreRol { get; set; } = null!;

    // Descripción del rol
    public string? Descripcion { get; set; }

    // Relación con los usuarios del sistema
    public virtual ICollection<Usuario> Usuarios { get; set; } =
        new List<Usuario>();
}