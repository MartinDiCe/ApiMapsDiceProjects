using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiMaps.Configuration;
using ApiMaps.Data;
using ApiMaps.Exceptions;
using ApiMaps.Helpers;
using ApiMaps.Middleware;
using ApiMaps.Models.Repositories.ApiMapConfigRepositories;
using ApiMaps.Models.Repositories.ApiTraceRepositories;
using ApiMaps.Models.Repositories.ParameterRepositories;
using ApiMaps.Services.ApiMapConfigServices;
using ApiMaps.Services.ApiTraceServices;
using ApiMaps.Services.AuditServices;
using ApiMaps.Services.GeocodingServices;
using ApiMaps.Services.LoggingServices;
using ApiMaps.Services.ParameterServices;
using ApiMaps.Services.ProviderConfigServices;
using ApiMaps.Traces;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1) Configurar servicios básicos (Controllers, Swagger, etc.)
// ---------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSwaggerConfiguration();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context => ModelValidationResponseFactory.CustomResponse(context.ModelState);
});

// ---------------------------------------------------------
// 2) Configurar base de datos y DbContext
// ---------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------------------------------------------------
// 3) Habilitar acceso al contexto HTTP (IHttpContextAccessor)
// ---------------------------------------------------------
builder.Services.AddHttpContextAccessor();

// ---------------------------------------------------------
// 4) Registrar servicios y utilidades personalizadas
// ---------------------------------------------------------
builder.Services.AddHttpClient();
builder.Services.AddScoped<IServiceExecutor, ReactiveServiceExecutor>();
builder.Services.AddScoped<AuditEntitiesService>();
builder.Services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));
builder.Services.AddScoped<IParameterRepository, ParameterRepository>();
builder.Services.AddScoped<IApiTraceRepository, ApiTraceRepository>();
builder.Services.AddScoped<IApiMapConfigRepository, ApiMapConfigRepository>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IApiTraceService, ApiTraceService>();
builder.Services.AddScoped<IApiMapConfigService, ApiMapConfigService>();
builder.Services.AddScoped<IProviderConfigurationService, ProviderConfigurationService>();

// Registrar la fábrica para crear dinámicamente proveedores de geocodificación.
// La fábrica se encarga de obtener la configuración desde la base de datos y construir las instancias
// de GeocodingProviderService con los valores correspondientes (endpoint, apiKey, priority, etc.).
builder.Services.AddScoped<IGeocodingProviderFactory, GeocodingProviderFactory>();

// Registrar el agregador de proveedores de geocodificación.
// Este servicio se encarga de combinar los resultados de los proveedores individuales (ya creados)
// según la prioridad o por proveedor.
builder.Services.AddScoped<IGeocodingProviderAggregatorService, GeocodingProviderAggregatorService>();

// Registrar el servicio central de geocodificación, que orquesta las operaciones invocando al agregador.
builder.Services.AddScoped<IGeocodeService, GeocodeService>();

builder.Services.AddMemoryCache();

// ---------------------------------------------------------
// 5) Configurar logging de forma condicional
// ---------------------------------------------------------
if (builder.Environment.IsProduction())
{
    builder.Logging.ClearProviders();
}
else
{
    // En desarrollo, se agregan los proveedores para consola y debug, con un nivel detallado.
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}


var app = builder.Build();

// ---------------------------------------------------------
// 7) Inicializar la base de datos (semillas, migraciones, etc.)
// ---------------------------------------------------------
DatabaseInitializer.Initialize(app.Services);

// ---------------------------------------------------------
// 8) Pipeline de Middlewares
// ---------------------------------------------------------

// En entornos de desarrollo, usar Swagger y el middleware de logging de request/response.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
}

// Registramos ExceptionMiddleware.
app.UseMiddleware<ApiTraceMiddleware>();

// Middleware para captura global de excepciones.
app.UseMiddleware<ExceptionMiddleware>();

// Iniciar el suscriptor reactivo para las trazas.
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
ApiTraceBus.StartTraceSubscriber(scopeFactory);

// Middleware de autorización (si es necesario).
app.UseAuthorization();

// Enruta las peticiones a los endpoints de los Controllers.
app.MapControllers();

// Arranca la aplicación.
app.Run();
