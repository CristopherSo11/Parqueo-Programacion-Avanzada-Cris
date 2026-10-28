using Microsoft.EntityFrameworkCore;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.BusinessLogic.Repositories.PostgreSQL;
using Parqueo.Model.Context;
using Parqueo.BusinessLogic.Repositories.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Habilitar los controladores de nuestra API.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// Leer la conexión PostgreSQL desde User Secrets.
var conexion = builder.Configuration.GetConnectionString("ParqueoDB");

if (string.IsNullOrWhiteSpace(conexion))
{
    throw new InvalidOperationException(
        "No se encontró la conexión ParqueoDB en User Secrets.");
}

// Configurar Entity Framework Core con PostgreSQL.
builder.Services.AddDbContext<ParqueoDbContext>(options =>
    options.UseNpgsql(conexion));
var proveedorParqueos =
    builder.Configuration["ProveedorParqueos"] ?? "PostgreSQL";

if (proveedorParqueos.Equals(
    "JSON", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IParqueoRepository>(sp =>
    {
        var rutaArchivo = Path.Combine(
            builder.Environment.ContentRootPath,
            "..",
            "DatosPruebas",
            "parqueos.json");

        return new ParqueoJsonRepository(
            Path.GetFullPath(rutaArchivo));
    });
}
else
{
    builder.Services.AddScoped<IParqueoRepository, ParqueoPostgresRepository>();
}
// Registrar nuestro repositorio de automóviles.
var proveedorAutomoviles =
    builder.Configuration["ProveedorAutomoviles"] ?? "PostgreSQL";

if (proveedorAutomoviles.Equals(
    "JSON", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IAutomovilRepository>(sp =>
    {
        var rutaArchivo = Path.Combine(
            builder.Environment.ContentRootPath,
            "..",
            "DatosPruebas",
            "automoviles.json");

        return new AutomovilJsonRepository(
            Path.GetFullPath(rutaArchivo));
    });
}
else
{
    builder.Services.AddScoped<IAutomovilRepository,
        AutomovilPostgresRepository>();
}
builder.Services.AddScoped<IIngresoAutomovilRepository, IngresoPostgresRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
