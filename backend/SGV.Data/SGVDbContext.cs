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
    public DbSet<AttendanceSheet> AttendanceSheets => Set<AttendanceSheet>();
    public DbSet<AttendanceSheetRow> AttendanceSheetRows => Set<AttendanceSheetRow>();
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

        modelBuilder.Entity<AttendanceSheet>(entity =>
        {
            entity.HasOne(s => s.SecurityGuard)
                  .WithMany()
                  .HasForeignKey(s => s.SecurityGuardId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Workplace)
                  .WithMany()
                  .HasForeignKey(s => s.WorkplaceId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.Rows)
                  .WithOne(r => r.AttendanceSheet)
                  .HasForeignKey(r => r.AttendanceSheetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.Year, s.Month, s.SecurityGuardId }).IsUnique();
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

        // PayrollConfig — Seed configuración inicial del sistema
        // Valores placeholder (> 0) para que la app funcione desde el primer deploy.
        // El usuario los actualiza desde la pantalla "Horas Extras → Actualizar Valor".
        modelBuilder.Entity<PayrollConfig>(entity =>
        {
            entity.HasData(new PayrollConfig
            {
                Id = 1,
                NormalHourRate = 1000m,
                NightSurchargeRate = 0m,
                HolidayHourRate = 1500m,
                ExtraHourRate = 1500m,
                ValidFrom = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Reason = "Configuración inicial del sistema",
                ChangedBy = "Sistema"
            });
        });
    }
}
