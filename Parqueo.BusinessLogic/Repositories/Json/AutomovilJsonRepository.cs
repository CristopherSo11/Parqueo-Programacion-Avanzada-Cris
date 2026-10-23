using System.Text.Json;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Repositories.Json;

public class AutomovilJsonRepository : IAutomovilRepository
{
    private readonly string _rutaArchivo;

    private readonly JsonSerializerOptions _opciones = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AutomovilJsonRepository(string rutaArchivo)
    {
        _rutaArchivo = rutaArchivo;
    }

    private async Task<List<PrqAutomovil>> LeerArchivoAsync()
    {
        if (!File.Exists(_rutaArchivo))
            throw new FileNotFoundException(
                "No se encontro el archivo de automoviles.",
                _rutaArchivo);

        string contenido = await File.ReadAllTextAsync(_rutaArchivo);

        return JsonSerializer.Deserialize<List<PrqAutomovil>>(
            contenido, _opciones) ?? new List<PrqAutomovil>();
    }

    public async Task<List<PrqAutomovil>> ObtenerTodosAsync()
    {
        return await LeerArchivoAsync();
    }

    public async Task<PrqAutomovil?> ObtenerPorIdAsync(int id)
    {
        var automoviles = await LeerArchivoAsync();

        return automoviles.FirstOrDefault(a => a.Id == id);
    }

    public async Task<List<PrqAutomovil>> FiltrarAsync(
        string? color = null,
        int? anioDesde = null,
        int? anioHasta = null,
        string? fabricante = null,
        string? tipo = null)
    {
        var automoviles = await LeerArchivoAsync();

        return automoviles.Where(a =>
            (string.IsNullOrWhiteSpace(color) ||
                a.Color.Contains(
                    color, StringComparison.OrdinalIgnoreCase)) &&

            (!anioDesde.HasValue || a.Anio >= anioDesde.Value) &&

            (!anioHasta.HasValue || a.Anio <= anioHasta.Value) &&

            (string.IsNullOrWhiteSpace(fabricante) ||
                a.Fabricante.Contains(
                    fabricante, StringComparison.OrdinalIgnoreCase)) &&

            (string.IsNullOrWhiteSpace(tipo) ||
                a.Tipo.Contains(
                    tipo, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public Task<PrqAutomovil> InsertarAsync(PrqAutomovil automovil)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }

    public Task<bool> ActualizarAsync(PrqAutomovil automovil)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }

    public Task<bool> EliminarAsync(int id)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }
}
