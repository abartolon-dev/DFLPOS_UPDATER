using DflPosUpdater.App.Entities;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<VersionApp> Versiones => Set<VersionApp>();
    public DbSet<VersionArchivo> VersionArchivos => Set<VersionArchivo>();
    public DbSet<Sucursal> Sucursales => Set<Sucursal>();
    public DbSet<Despliegue> Despliegues => Set<Despliegue>();
    public DbSet<DespliegueLog> DespliegueLogs => Set<DespliegueLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VersionApp>(entity =>
        {
            entity.ToTable("Versiones");
            entity.HasIndex(x => x.NumeroVersion).IsUnique();
            entity.Property(x => x.Estado).HasConversion<string>().HasMaxLength(30);
            entity.Property(x => x.FechaCreacion).HasDefaultValueSql("GETDATE()");
            entity.Property(x => x.TotalArchivos).HasDefaultValue(0);
            entity.Property(x => x.RutaCarpeta).HasDefaultValue("");
        });

        modelBuilder.Entity<VersionArchivo>(entity =>
        {
            entity.ToTable("VersionArchivos");
            entity.HasIndex(x => new { x.VersionAppId, x.RutaRelativa }).IsUnique();
            entity.Property(x => x.Activo).HasDefaultValue(true);
            entity.Property(x => x.FechaCarga).HasDefaultValueSql("GETDATE()");

            entity.HasOne(x => x.VersionApp)
                .WithMany(x => x.Archivos)
                .HasForeignKey(x => x.VersionAppId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.ToTable("Sucursales");
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.Property(x => x.Activa).HasDefaultValue(true);
            entity.Property(x => x.PuertoFtp).HasDefaultValue(21);
            entity.Property(x => x.RutaDestino).HasDefaultValue("/updates");
        });

        modelBuilder.Entity<Despliegue>(entity =>
        {
            entity.ToTable("Despliegues");
            entity.Property(x => x.Estado).HasConversion<string>().HasMaxLength(30);
            entity.Property(x => x.FechaCreacion).HasDefaultValueSql("GETDATE()");

            entity.HasOne(x => x.VersionApp)
                .WithMany(x => x.Despliegues)
                .HasForeignKey(x => x.VersionAppId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Sucursal)
                .WithMany(x => x.Despliegues)
                .HasForeignKey(x => x.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.VersionAppId, x.SucursalId }).IsUnique();
        });

        modelBuilder.Entity<DespliegueLog>(entity =>
        {
            entity.ToTable("DespliegueLogs");
            entity.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(30);
            entity.Property(x => x.Fecha).HasDefaultValueSql("GETDATE()");

            entity.HasOne(x => x.Despliegue)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.DespliegueId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
