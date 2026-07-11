/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Modelo que representa a los usuarios del sistema y su
 *               relación con roles, reportes e historial de eventos.
 **************************************************************************/

namespace CyberWatchAnalytics.Models;

public partial class Usuario
{
    // Clave primaria
    public int IdUsuario { get; set; }

    // Rol asignado al usuario
    public int IdRol { get; set; }

    // Información básica del usuario
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;

    // Contraseña (temporalmente almacenada en texto para el proyecto)
    public string PasswordHash { get; set; } = null!;

    // Estado del usuario (Activo / Inactivo)
    public string Estado { get; set; } = null!;

    // Fecha de creación del registro
    public DateTime FechaCreacion { get; set; }

    // Relaciones con otras entidades
    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<HistorialEvento> HistorialEventos { get; set; } =
        new List<HistorialEvento>();

    public virtual ICollection<Reporte> Reportes { get; set; } =
        new List<Reporte>();
}