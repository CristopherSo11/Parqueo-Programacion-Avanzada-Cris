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

private async Task GuardarArchivoAsync(
    List<PrqIngresoAutomovil> ingresos)
{
    var datosParaGuardar = ingresos.Select(i => new
    {
        consecutivo = i.Consecutivo,
        id_parqueo = i.IdParqueo,
        id_automovil = i.IdAutomovil,
        fecha_hora_entrada = i.FechaHoraEntrada,
        fecha_hora_salida = i.FechaHoraSalida
    }).ToList();

    var opcionesEscritura = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    string contenido = JsonSerializer.Serialize(
        datosParaGuardar,
        opcionesEscritura);

    await File.WriteAllTextAsync(
        _rutaIngresos,
        contenido);
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


public async Task<PrqIngresoAutomovil> InsertarAsync(
    PrqIngresoAutomovil ingreso)
{
    var ingresos = await LeerArchivoAsync();

    var automovil = await _automovilRepository.ObtenerPorIdAsync(
        ingreso.IdAutomovil);

    var parqueo = await _parqueoRepository.ObtenerPorIdAsync(
        ingreso.IdParqueo);

    if (automovil == null || parqueo == null)
    {
        throw new ArgumentException(
            "El automovil o el parqueo indicado no existe.");
    }

    if (ingreso.FechaHoraSalida.HasValue &&
        ingreso.FechaHoraSalida.Value < ingreso.FechaHoraEntrada)
    {
        throw new ArgumentException(
            "La fecha de salida no puede ser anterior a la entrada.");
    }

    ingreso.Consecutivo = ingresos.Count == 0
        ? 1
        : ingresos.Max(i => i.Consecutivo) + 1;

    ingreso.IdAutomovilNavigation = automovil;
    ingreso.IdParqueoNavigation = parqueo;

    ingresos.Add(ingreso);

    await GuardarArchivoAsync(ingresos);

    return ingreso;
}

public async Task<bool> ActualizarAsync(
    PrqIngresoAutomovil ingreso)
{
    var ingresos = await LeerArchivoAsync();

    var existente = ingresos.FirstOrDefault(
        i => i.Consecutivo == ingreso.Consecutivo);

    if (existente == null)
    {
        return false;
    }

    var automovil = await _automovilRepository.ObtenerPorIdAsync(
        ingreso.IdAutomovil);

    var parqueo = await _parqueoRepository.ObtenerPorIdAsync(
        ingreso.IdParqueo);

    if (automovil == null || parqueo == null)
    {
        throw new ArgumentException(
            "El automovil o el parqueo indicado no existe.");
    }

    if (ingreso.FechaHoraSalida.HasValue &&
        ingreso.FechaHoraSalida.Value < ingreso.FechaHoraEntrada)
    {
        throw new ArgumentException(
            "La fecha de salida no puede ser anterior a la entrada.");
    }

    existente.IdAutomovil = ingreso.IdAutomovil;
    existente.IdParqueo = ingreso.IdParqueo;
    existente.FechaHoraEntrada = ingreso.FechaHoraEntrada;
    existente.FechaHoraSalida = ingreso.FechaHoraSalida;

    existente.IdAutomovilNavigation = automovil;
    existente.IdParqueoNavigation = parqueo;

    await GuardarArchivoAsync(ingresos);

    return true;
}

public async Task<bool> EliminarAsync(int consecutivo)
{
    var ingresos = await LeerArchivoAsync();

    var existente = ingresos.FirstOrDefault(
        i => i.Consecutivo == consecutivo);

    if (existente == null)
    {
        return false;
    }

    ingresos.Remove(existente);

    await GuardarArchivoAsync(ingresos);

    return true;
}

}
