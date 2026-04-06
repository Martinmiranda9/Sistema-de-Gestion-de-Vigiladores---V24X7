using Microsoft.EntityFrameworkCore;
using SGV.Entities;

namespace SGV.Data;

public class SGVDbContext : DbContext
{
    public SGVDbContext(DbContextOptions<SGVDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Vigilador> Vigiladores => Set<Vigilador>();
    public DbSet<Objetivo> Objetivos => Set<Objetivo>();
    public DbSet<Feriado> Feriados => Set<Feriado>();
    public DbSet<RegistroTurno> RegistroTurnos => Set<RegistroTurno>();
    public DbSet<ConfiguracionLiquidacion> ConfiguracionesLiquidacion => Set<ConfiguracionLiquidacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Vigilador - índice único en DNI y relación con Objetivo
        modelBuilder.Entity<Vigilador>(entity =>
        {
            entity.HasIndex(v => v.DNI).IsUnique();
            
            entity.HasOne(v => v.Objetivo)
                  .WithMany()
                  .HasForeignKey(v => v.ObjetivoId)
                  .OnDelete(DeleteBehavior.SetNull); // Default behaviour or whatever works
        });

        // RegistroTurno - relación con Vigilador
        modelBuilder.Entity<RegistroTurno>(entity =>
        {
            entity.HasOne(rt => rt.Vigilador)
                  .WithMany()
                  .HasForeignKey(rt => rt.VigiladorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Aplicar configuraciones adicionales del assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SGVDbContext).Assembly);
    }
}
