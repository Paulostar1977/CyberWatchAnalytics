/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Contexto de base de datos utilizado por Entity Framework
 *               Core para acceder a las tablas del sistema.
 **************************************************************************/

using CyberWatchAnalytics.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberWatchAnalytics.Data;

public partial class CyberWatchDbContext : DbContext
{
    public CyberWatchDbContext(DbContextOptions<CyberWatchDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivosTecnologico> ActivosTecnologicos { get; set; }

    public virtual DbSet<HistorialEvento> HistorialEventos { get; set; }

    public virtual DbSet<IncidentesSeguridad> IncidentesSeguridads { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<TiposActivo> TiposActivos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vulnerabilidad> Vulnerabilidades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivosTecnologico>(entity =>
        {
            entity.HasKey(e => e.IdActivo);

            entity.Property(e => e.DireccionIp)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("DireccionIP");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Activo");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.NombreActivo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Ubicacion)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTipoActivoNavigation)
                .WithMany(p => p.ActivosTecnologicos)
                .HasForeignKey(d => d.IdTipoActivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activos_TiposActivo");
        });

        modelBuilder.Entity<HistorialEvento>(entity =>
        {
            entity.HasKey(e => e.IdEvento);

            entity.Property(e => e.Accion)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.FechaEvento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.HistorialEventos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Historial_Usuarios");
        });

        modelBuilder.Entity<IncidentesSeguridad>(entity =>
        {
            entity.HasKey(e => e.IdIncidente);

            entity.ToTable("IncidentesSeguridad");

            entity.Property(e => e.Criticidad)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Abierto");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Titulo)
                .HasMaxLength(120)
                .IsUnicode(false);

            entity.HasOne(d => d.IdActivoNavigation)
                .WithMany(p => p.IncidentesSeguridads)
                .HasForeignKey(d => d.IdActivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidentes_Activos");
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.HasKey(e => e.IdReporte);

            entity.Property(e => e.FechaGeneracion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.TipoReporte)
                .HasMaxLength(80)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Reportes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reportes_Usuarios");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TiposActivo>(entity =>
        {
            entity.HasKey(e => e.IdTipoActivo);

            entity.ToTable("TiposActivo");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.NombreTipo)
                .HasMaxLength(80)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.HasIndex(e => e.Correo).IsUnique();

            entity.Property(e => e.CodigoRecuperacion)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Activo");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.FechaExpiracionCodigo)
                .HasColumnType("datetime");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");
        });

        modelBuilder.Entity<Vulnerabilidad>(entity =>
        {
            entity.HasKey(e => e.IdVulnerabilidad);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.EstadoMitigacion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");

            entity.Property(e => e.FechaDeteccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Severidad)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdActivoNavigation)
                .WithMany(p => p.Vulnerabilidades)
                .HasForeignKey(d => d.IdActivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vulnerabilidades_Activos");
        });
    }
}