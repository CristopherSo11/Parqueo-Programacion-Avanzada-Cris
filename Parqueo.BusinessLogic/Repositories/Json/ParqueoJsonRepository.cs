using System.Text.Json;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Repositories.Json;

public class ParqueoJsonRepository : IParqueoRepository
{
    private readonly string _rutaArchivo;

    private readonly JsonSerializerOptions _opciones = new()
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
};

    public ParqueoJsonRepository(string rutaArchivo)
    {
        _rutaArchivo = rutaArchivo;
    }

    private async Task<List<PrqParqueo>> LeerArchivoAsync()
    {
        if (!File.Exists(_rutaArchivo))
        {
            throw new FileNotFoundException(
                "No se encontro el archivo de parqueos.",
                _rutaArchivo);
        }

        string contenido = await File.ReadAllTextAsync(_rutaArchivo);

        return JsonSerializer.Deserialize<List<PrqParqueo>>(
            contenido, _opciones) ?? new List<PrqParqueo>();
    }

    public async Task<List<PrqParqueo>> ObtenerTodosAsync()
    {
        return await LeerArchivoAsync();
    }

    public async Task<PrqParqueo?> ObtenerPorIdAsync(int id)
    {
        var parqueos = await LeerArchivoAsync();

        return parqueos.FirstOrDefault(p => p.Id == id);
    }

    public async Task<List<PrqParqueo>> FiltrarAsync(
        string? provincia = null,
        string? nombre = null,
        decimal? precioMinimo = null,
        decimal? precioMaximo = null)
    {
        var parqueos = await LeerArchivoAsync();

        return parqueos.Where(p =>
            (string.IsNullOrWhiteSpace(provincia) ||
                p.NombreProvincia.Contains(
                    provincia,
                    StringComparison.OrdinalIgnoreCase)) &&

            (string.IsNullOrWhiteSpace(nombre) ||
                p.Nombre.Contains(
                    nombre,
                    StringComparison.OrdinalIgnoreCase)) &&

            (!precioMinimo.HasValue ||
                p.PrecioPorHora >= precioMinimo.Value) &&

            (!precioMaximo.HasValue ||
                p.PrecioPorHora <= precioMaximo.Value)
        ).ToList();
    }

    public async Task<decimal?> ObtenerPrecioPorHoraPorParqueo(int id)
    {
        var parqueo = await ObtenerPorIdAsync(id);

        return parqueo?.PrecioPorHora;
    }

    public Task<PrqParqueo> InsertarAsync(PrqParqueo parqueo)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }

    public Task<bool> ActualizarAsync(PrqParqueo parqueo)
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
