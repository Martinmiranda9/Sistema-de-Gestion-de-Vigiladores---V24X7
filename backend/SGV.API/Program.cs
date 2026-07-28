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

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Entity Framework Core — PostgreSQL
builder.Services.AddDbContext<SGVDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SGV.Data")
    )
);

// CORS — orígenes permitidos desde configuración
// En producción (Render) setear: AllowedOrigins=https://tu-app.vercel.app
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(",", StringSplitOptions.RemoveEmptyEntries)
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins)
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
builder.Services.AddScoped<IOvertimeSpreadsheetRepository, OvertimeSpreadsheetRepository>();
builder.Services.AddScoped<IAttendanceSheetRepository, AttendanceSheetRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services (Application)
builder.Services.AddScoped<ISecurityGuardService, SecurityGuardService>();
builder.Services.AddScoped<IWorkplaceService, WorkplaceService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<IShiftRecordService, ShiftRecordService>();
builder.Services.AddScoped<IPayrollConfigService, PayrollConfigService>();
builder.Services.AddScoped<IOvertimeSpreadsheetService, OvertimeSpreadsheetService>();
builder.Services.AddScoped<IAttendanceSheetService, AttendanceSheetService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Gemini Service + HttpClient
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "default_super_secret_key_12345";
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });

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

// CORS debe ir primero — antes de cualquier redirect o auth
// Render maneja SSL externamente, no se necesita UseHttpsRedirection
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
