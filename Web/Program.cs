using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Entity.Context;
using Data.Interfaces;
using Data.Implements.RolData;
using Data.Implements.RolUserData;
using Data.Implements.UserDate;
using Data.Implements.CityData;
using Data.Implements.ClientData;
using Data.Implements.CountryData;
using Data.Implements.DepartmentData;
using Data.Implements.EmployeeData;
using Data.Implements.FormData;
using Data.Implements.ModuleData;
using Data.Implements.ModuleFormData;
using Data.Implements.NeighborhoodData;
using Data.Implements.PermissionData;
using Data.Implements.PersonData;
using Data.Implements.ProviderData;
using Data.Implements.RolFormPermissionData;
using Data.Implements.BaseData;

using Business.Interfaces;
using Business.Implements;
using Business.Services;

using Utilities.Interfaces;
using Utilities.Helpers;
using Utilities.Mail;
using Utilities.Jwt;
using Utilities.Services;

using Web.ServiceExtension;
using FluentValidation;
using FluentValidation.AspNetCore;
using Business.Interfaces.Business.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==================== SERVICIOS BÁSICOS ====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Mantener nombres originales
        options.JsonSerializerOptions.WriteIndented = true; // JSON más legible en development
    });

// Add HttpContextAccessor para auditoría
builder.Services.AddHttpContextAccessor();

// ==================== VALIDACIÓN ====================
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddSingleton<IValidatorFactory, ServiceProviderValidatorFactory>();
builder.Services.AddFluentValidationAutoValidation();

// ==================== DOCUMENTACIÓN API ====================
builder.Services.AddSwaggerDocumentation();

// ==================== BASE DE DATOS ====================
// DbContext principal
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// DbContext de auditoría
builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditConnection")));

// ==================== AUTOMAPPER ====================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ==================== SERVICIOS DE EMAIL ====================
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// ==================== JWT Y AUTENTICACIÓN ====================
builder.Services.AddScoped<IJwtGenerator, GenerateTokenJwt>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Configurar autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes(
        builder.Configuration["JWT:Key"] ??
        throw new InvalidOperationException("JWT:Key is not configured"));

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Configurar eventos para debugging en desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    }
});

// ==================== SERVICIOS DE AUDITORÍA ====================
builder.Services.AddScoped<IAuditService, AuditService>();

// ==================== REPOSITORIOS BASE ====================
builder.Services.AddScoped(typeof(IBaseModelData<>), typeof(BaseModelData<>));
builder.Services.AddScoped(typeof(IBaseBusiness<,>), typeof(BaseBusiness<,>));

// ==================== REPOSITORIOS ESPECÍFICOS ====================
// Usuario y autenticación
builder.Services.AddScoped<IUserData, UserData>();
builder.Services.AddScoped<IRolData, RolData>();
builder.Services.AddScoped<IRolUserData, RolUserData>();

// Entidades geográficas
builder.Services.AddScoped<ICountryData, CountryData>();
builder.Services.AddScoped<IDepartmentData, DepartmentData>();
builder.Services.AddScoped<ICityData, CityData>();
builder.Services.AddScoped<INeighborhoodData, NeighborhoodData>();

// Entidades de personas
builder.Services.AddScoped<IPersonData, PersonData>();
builder.Services.AddScoped<IClientData, ClientData>();
builder.Services.AddScoped<IProviderData, ProviderData>();
builder.Services.AddScoped<IEmployeeData, EmployeeData>();

// Entidades del sistema
builder.Services.AddScoped<IModuleData, ModuleData>();
builder.Services.AddScoped<IFormData, FormData>();
builder.Services.AddScoped<IModuleFormData, ModuleFormData>();
builder.Services.AddScoped<IPermissionData, PermissionData>();
builder.Services.AddScoped<IRolFormPermissionData, RolFormPermissionData>();

// ==================== SERVICIOS DE NEGOCIO ====================
// Usuario y autenticación
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IRolBusiness, RolBusiness>();
builder.Services.AddScoped<IRoleUserBusiness, RoleUserBusiness>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Entidades geográficas
builder.Services.AddScoped<ICountryBusiness, CountryBusiness>();
builder.Services.AddScoped<IDepartmentBusiness, DepartmentBusiness>();
builder.Services.AddScoped<ICityBusiness, CityBusiness>();
builder.Services.AddScoped<INeighborhoodBusiness, NeighborhoodBusiness>();

// Entidades de personas
builder.Services.AddScoped<IPersonBusiness, PersonBusiness>();
builder.Services.AddScoped<IClientBusiness, ClientBusiness>();
builder.Services.AddScoped<IProviderBusiness, ProviderBusiness>();
builder.Services.AddScoped<IEmployeeBusiness, EmployeeBusiness>();

// Entidades del sistema
builder.Services.AddScoped<IModuleBusiness, ModuleBusiness>();
builder.Services.AddScoped<IFormBusiness, FormBusiness>();
builder.Services.AddScoped<IModuleFormBusiness, ModuleFormBusiness>();
builder.Services.AddScoped<IPermissionBusiness, PermissionBusiness>();
builder.Services.AddScoped<IRolFormPermissionBusiness, RolFormPermissionBusiness>();

// ==================== HELPERS Y UTILIDADES ====================
builder.Services.AddScoped<IGenericIHelpers, GenericHelpers>();
builder.Services.AddScoped<IDatetimeHelper, DatetimeHelper>();
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
builder.Services.AddScoped<IAuthHeaderHelper, AuthHeaderHelper>();
builder.Services.AddScoped<IRoleHelper, RoleHelper>();
builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<IValidationHelper, ValidationHelper>();

// ==================== CORS ====================
var origenesPermitidos = builder.Configuration["OrigenesPermitidos"]?.Split(";") ??
    new[] { "http://localhost:4200", "https://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder
            .WithOrigins(origenesPermitidos)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Para permitir cookies y headers de auth
    });

    // Política más restrictiva para producción
    options.AddPolicy("Production", corsBuilder =>
    {
        corsBuilder
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                        new[] { "https://yourdomain.com" })
            .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials();
    });
});

// ==================== CACHE ====================
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

// ==================== HEALTH CHECKS ====================


builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("DefaultConnection")
    .AddDbContextCheck<AuditDbContext>("AuditConnection");





// ==================== LOGGING ====================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}

// ==================== BUILD APPLICATION ====================
var app = builder.Build();

// ==================== CONFIGURACIÓN DEL PIPELINE ====================
// Health checks
app.MapHealthChecks("/health");

// Desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Sistema de Gestión v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
        c.DisplayRequestDuration(); // Mostrar tiempo de respuesta
    });
}
else
{
    // Producción
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

// CORS
app.UseCors();

// HTTPS redirection
app.UseHttpsRedirection();

// Response caching
app.UseResponseCaching();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// ==================== INICIALIZACIÓN DE BASE DE DATOS ====================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("🚀 Iniciando aplicación...");

        // Migrar base de datos principal
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("📊 Aplicando migraciones a la base de datos principal...");

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
            logger.LogInformation("✅ Base de datos principal actualizada exitosamente");
        }
        else
        {
            logger.LogInformation("ℹ️ Base de datos principal ya está actualizada");
        }

        // Migrar base de datos de auditoría
        var auditContext = services.GetRequiredService<AuditDbContext>();
        logger.LogInformation("📋 Aplicando migraciones a la base de datos de auditoría...");

        if (auditContext.Database.GetPendingMigrations().Any())
        {
            auditContext.Database.Migrate();
            logger.LogInformation("✅ Base de datos de auditoría actualizada exitosamente");
        }
        else
        {
            logger.LogInformation("ℹ️ Base de datos de auditoría ya está actualizada");
        }

        // Verificar conexiones
        if (await dbContext.Database.CanConnectAsync())
        {
            logger.LogInformation("✅ Conexión a base de datos principal verificada");
        }

        if (await auditContext.Database.CanConnectAsync())
        {
            logger.LogInformation("✅ Conexión a base de datos de auditoría verificada");
        }

        logger.LogInformation("🎉 Aplicación iniciada correctamente");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error crítico durante la inicialización de las bases de datos");
        throw; // Re-lanzar para que la aplicación no inicie con BD inconsistente
    }
}

// ==================== EJECUTAR APLICACIÓN ====================
app.Run();