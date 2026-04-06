using SGV.Data;
using SGV.Business.Interfaces;
using SGV.Business.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Servicios =====

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SGV API",
        Version = "v1",
        Description = "API del Sistema de Gestión de Vigiladores"
    });
});

// Entity Framework Core - SQL Server
builder.Services.AddDbContext<SGVDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SGV.Data")
    )
);

// CORS - para permitir conexión desde Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Inyección de dependencias - Capa Business
builder.Services.AddScoped<IVigiladorService, VigiladorService>();
builder.Services.AddScoped<IFeriadoService, FeriadoService>();
builder.Services.AddScoped<IRegistroTurnoService, RegistroTurnoService>();
builder.Services.AddScoped<IConfiguracionLiquidacionService, ConfiguracionLiquidacionService>();
builder.Services.AddScoped<IObjetivoService, ObjetivoService>();

var app = builder.Build();

// ===== Middleware Pipeline =====

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGV API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
