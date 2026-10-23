using System.Text.Json;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Repositories.Json;

public class IngresoJsonRepository : IIngresoAutomovilRepository
{
    private readonly string _rutaIngresos;
    private readonly IAutomovilRepository _automovilRepository;
    private readonly IParqueoRepository _parqueoRepository;

    private readonly JsonSerializerOptions _opciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public IngresoJsonRepository(
        string rutaIngresos,
        IAutomovilRepository automovilRepository,
        IParqueoRepository parqueoRepository)
    {
        _rutaIngresos = rutaIngresos;
        _automovilRepository = automovilRepository;
        _parqueoRepository = parqueoRepository;
    }

    private async Task<List<PrqIngresoAutomovil>> LeerArchivoAsync()
    {
        if (!File.Exists(_rutaIngresos))
        {
            throw new FileNotFoundException(
                "No se encontro el archivo de ingresos.",
                _rutaIngresos);
        }

        string contenido = await File.ReadAllTextAsync(_rutaIngresos);

        var ingresos =
            JsonSerializer.Deserialize<List<PrqIngresoAutomovil>>(
                contenido, _opciones)
            ?? new List<PrqIngresoAutomovil>();

        var automoviles = await _automovilRepository.ObtenerTodosAsync();
        var parqueos = await _parqueoRepository.ObtenerTodosAsync();

        foreach (var ingreso in ingresos)
        {
            ingreso.IdAutomovilNavigation =
                automoviles.FirstOrDefault(
                    a => a.Id == ingreso.IdAutomovil)!;

            ingreso.IdParqueoNavigation =
                parqueos.FirstOrDefault(
                    p => p.Id == ingreso.IdParqueo)!;
        }

        return ingresos;
    }

    public async Task<List<PrqIngresoAutomovil>> ObtenerTodosAsync()
    {
        return await LeerArchivoAsync();
    }

    public async Task<PrqIngresoAutomovil?> ObtenerPorIdAsync(
        int consecutivo)
    {
        var ingresos = await LeerArchivoAsync();

        return ingresos.FirstOrDefault(
            i => i.Consecutivo == consecutivo);
    }

    public async Task<List<PrqIngresoAutomovil>>
        FiltrarPorTipoYFechasAsync(
            string? tipo,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
    {
        var ingresos = await LeerArchivoAsync();

        return ingresos.Where(i =>
            (string.IsNullOrWhiteSpace(tipo) ||
                (i.IdAutomovilNavigation?.Tipo.Contains(
                    tipo,
                    StringComparison.OrdinalIgnoreCase) ?? false)) &&

            (!fechaDesde.HasValue ||
                i.FechaHoraEntrada >= fechaDesde.Value) &&

            (!fechaHasta.HasValue ||
                i.FechaHoraEntrada <= fechaHasta.Value)
        ).ToList();
    }

    public async Task<List<PrqIngresoAutomovil>>
        FiltrarPorProvinciaYFechasAsync(
            string? provincia,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
    {
        var ingresos = await LeerArchivoAsync();

        return ingresos.Where(i =>
            (string.IsNullOrWhiteSpace(provincia) ||
                (i.IdParqueoNavigation?.NombreProvincia.Contains(
                    provincia,
                    StringComparison.OrdinalIgnoreCase) ?? false)) &&

            (!fechaDesde.HasValue ||
                i.FechaHoraEntrada >= fechaDesde.Value) &&

            (!fechaHasta.HasValue ||
                i.FechaHoraEntrada <= fechaHasta.Value)
        ).ToList();
    }

    public Task<PrqIngresoAutomovil> InsertarAsync(
        PrqIngresoAutomovil ingreso)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }

    public Task<bool> ActualizarAsync(
        PrqIngresoAutomovil ingreso)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }

    public Task<bool> EliminarAsync(int consecutivo)
    {
        throw new NotImplementedException(
            "Pendiente para la etapa de CRUD.");
    }
}
