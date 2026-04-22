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
    public DbSet<OvertimeSpreadsheet> OvertimeSpreadsheets => Set<OvertimeSpreadsheet>();
    public DbSet<OvertimeSpreadsheetRow> OvertimeSpreadsheetRows => Set<OvertimeSpreadsheetRow>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SecurityGuard — unique index on NationalId, optional FK to Workplace
        modelBuilder.Entity<SecurityGuard>(entity =>
        {
            entity.HasIndex(g => g.DNI).IsUnique();

            entity.HasOne(g => g.Workplace)
                  .WithMany()
                  .HasForeignKey(g => g.WorkplaceId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ShiftRecord — FK to SecurityGuard (restricted) and Workplace (nullable)
        modelBuilder.Entity<ShiftRecord>(entity =>
        {
            entity.HasOne(r => r.SecurityGuard)
                  .WithMany()
                  .HasForeignKey(r => r.SecurityGuardId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Workplace)
                  .WithMany()
                  .HasForeignKey(r => r.WorkplaceId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OvertimeSpreadsheet>(entity =>
        {
            entity.HasOne(s => s.Workplace)
                  .WithMany()
                  .HasForeignKey(s => s.WorkplaceId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.Rows)
                  .WithOne(r => r.OvertimeSpreadsheet)
                  .HasForeignKey(r => r.OvertimeSpreadsheetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.Year, s.Month, s.WorkplaceId });
        });

        modelBuilder.Entity<OvertimeSpreadsheetRow>(entity =>
        {
            entity.HasOne(r => r.SecurityGuard)
                  .WithMany()
                  .HasForeignKey(r => r.SecurityGuardId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Users — Seed admin user
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();

            entity.HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true
            });
        });
    }
}
