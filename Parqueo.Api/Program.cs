using Microsoft.EntityFrameworkCore;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.BusinessLogic.Repositories.PostgreSQL;
using Parqueo.Model.Context;
using Parqueo.BusinessLogic.Repositories.Json;


var builder = WebApplication.CreateBuilder(args);

// Habilitar los controladores de nuestra API.
builder.Services.AddControllers();

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
builder.Services.AddScoped<IParqueoRepository, ParqueoPostgresRepository>();
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

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
