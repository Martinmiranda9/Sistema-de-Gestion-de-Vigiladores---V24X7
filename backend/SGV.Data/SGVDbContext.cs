using Microsoft.EntityFrameworkCore;
using SGV.Entities;

namespace SGV.Data;

public class SGVDbContext : DbContext
{
    public SGVDbContext(DbContextOptions<SGVDbContext> options) : base(options) { }

    public DbSet<SecurityGuard> SecurityGuards => Set<SecurityGuard>();
    public DbSet<Workplace> Workplaces => Set<Workplace>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<ShiftRecord> ShiftRecords => Set<ShiftRecord>();
    public DbSet<PayrollConfig> PayrollConfigs => Set<PayrollConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SecurityGuard — unique index on NationalId, optional FK to Workplace
        modelBuilder.Entity<SecurityGuard>(entity =>
        {
            entity.HasIndex(g => g.NationalId).IsUnique();

            entity.HasOne(g => g.Workplace)
                  .WithMany()
                  .HasForeignKey(g => g.WorkplaceId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ShiftRecord — FK to SecurityGuard, restricted delete to preserve history
        modelBuilder.Entity<ShiftRecord>(entity =>
        {
            entity.HasOne(r => r.SecurityGuard)
                  .WithMany()
                  .HasForeignKey(r => r.SecurityGuardId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
