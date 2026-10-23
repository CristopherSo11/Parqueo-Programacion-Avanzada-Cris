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


private async Task GuardarArchivoAsync(
    List<PrqAutomovil> automoviles)
{
    string contenido = JsonSerializer.Serialize(
        automoviles,
        new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

    await File.WriteAllTextAsync(_rutaArchivo, contenido);
}

public async Task<PrqAutomovil> InsertarAsync(
    PrqAutomovil automovil)
{
    var automoviles = await LeerArchivoAsync();

    automovil.Id = automoviles.Count == 0
        ? 1
        : automoviles.Max(a => a.Id) + 1;

    automoviles.Add(automovil);

    await GuardarArchivoAsync(automoviles);

    return automovil;
}

public async Task<bool> ActualizarAsync(
    PrqAutomovil automovil)
{
    var automoviles = await LeerArchivoAsync();

    var existente = automoviles.FirstOrDefault(
        a => a.Id == automovil.Id);

    if (existente == null)
    {
        return false;
    }

    existente.Color = automovil.Color;
    existente.Anio = automovil.Anio;
    existente.Fabricante = automovil.Fabricante;
    existente.Tipo = automovil.Tipo;

    await GuardarArchivoAsync(automoviles);

    return true;
}

public async Task<bool> EliminarAsync(int id)
{
    var automoviles = await LeerArchivoAsync();

    var existente = automoviles.FirstOrDefault(
        a => a.Id == id);

    if (existente == null)
    {
        return false;
    }

    automoviles.Remove(existente);

    await GuardarArchivoAsync(automoviles);

    return true;
}

}
