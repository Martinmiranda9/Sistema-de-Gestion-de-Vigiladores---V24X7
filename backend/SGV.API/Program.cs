using SGV.Data;
using SGV.Data.Repositories;
using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.Business.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SGV API",
        Version = "v1",
        Description = "Sistema de Gestión de Vigiladores — REST API"
    });
});

// Entity Framework Core — SQL Server
builder.Services.AddDbContext<SGVDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SGV.Data")
    )
);

// CORS — Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Repositories (Infrastructure)
builder.Services.AddScoped<ISecurityGuardRepository, SecurityGuardRepository>();
builder.Services.AddScoped<IWorkplaceRepository, WorkplaceRepository>();
builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
builder.Services.AddScoped<IShiftRecordRepository, ShiftRecordRepository>();
builder.Services.AddScoped<IPayrollConfigRepository, PayrollConfigRepository>();

// Services (Application)
builder.Services.AddScoped<ISecurityGuardService, SecurityGuardService>();
builder.Services.AddScoped<IWorkplaceService, WorkplaceService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IShiftRecordService, ShiftRecordService>();
builder.Services.AddScoped<IPayrollConfigService, PayrollConfigService>();

var app = builder.Build();

// ====== SE AGREGO ESTO PARA DOCKER ======
// Run DB migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SGVDbContext>();
    db.Database.Migrate();
}
// ========================================

// ===== Middleware Pipeline =====

// ====== SE AGREGO ESTO PARA DOCKER ======
// Habilitamos Swagger siempre
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGV API v1");
});
// ========================================

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
