using Microsoft.EntityFrameworkCore;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Context;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Repositories.PostgreSQL;

public class IngresoPostgresRepository : IIngresoAutomovilRepository
{
    private readonly ParqueoDbContext _context;

    public IngresoPostgresRepository(ParqueoDbContext context)
    {
        _context = context;
    }

    public async Task<List<PrqIngresoAutomovil>> ObtenerTodosAsync()
    {
        return await ConsultaConRelaciones()
            .ToListAsync();
    }

    public async Task<PrqIngresoAutomovil?> ObtenerPorIdAsync(
        int consecutivo)
    {
        return await ConsultaConRelaciones()
            .FirstOrDefaultAsync(i => i.Consecutivo == consecutivo);
    }

    public async Task<List<PrqIngresoAutomovil>> FiltrarPorTipoYFechasAsync(
        string? tipo,
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        var consulta = ConsultaConRelaciones();

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            consulta = consulta.Where(i =>
                EF.Functions.ILike(
                    i.IdAutomovilNavigation.Tipo,
                    $"%{tipo}%"));
        }

        if (fechaDesde.HasValue)
        {
            consulta = consulta.Where(i =>
                i.FechaHoraEntrada >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            consulta = consulta.Where(i =>
                i.FechaHoraEntrada <= fechaHasta.Value);
        }

        return await consulta.ToListAsync();
    }

    public async Task<List<PrqIngresoAutomovil>>
        FiltrarPorProvinciaYFechasAsync(
            string? provincia,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
    {
        var consulta = ConsultaConRelaciones();

        if (!string.IsNullOrWhiteSpace(provincia))
        {
            consulta = consulta.Where(i =>
                EF.Functions.ILike(
                    i.IdParqueoNavigation.NombreProvincia,
                    $"%{provincia}%"));
        }

        if (fechaDesde.HasValue)
        {
            consulta = consulta.Where(i =>
                i.FechaHoraEntrada >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            consulta = consulta.Where(i =>
                i.FechaHoraEntrada <= fechaHasta.Value);
        }

        return await consulta.ToListAsync();
    }

    private IQueryable<PrqIngresoAutomovil> ConsultaConRelaciones()
    {
        return _context.PrqIngresoAutomovils
            .AsNoTracking()
            .Include(i => i.IdAutomovilNavigation)
            .Include(i => i.IdParqueoNavigation);
    }


public async Task<PrqIngresoAutomovil> InsertarAsync(
    PrqIngresoAutomovil ingreso)
{
    bool existeAutomovil = await _context.PrqAutomovils
        .AnyAsync(a => a.Id == ingreso.IdAutomovil);

    bool existeParqueo = await _context.PrqParqueos
        .AnyAsync(p => p.Id == ingreso.IdParqueo);

    if (!existeAutomovil || !existeParqueo)
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

    await _context.PrqIngresoAutomovils.AddAsync(ingreso);
    await _context.SaveChangesAsync();

    return ingreso;
}

public async Task<bool> ActualizarAsync(
    PrqIngresoAutomovil ingreso)
{
    var existente = await _context.PrqIngresoAutomovils
        .FirstOrDefaultAsync(
            i => i.Consecutivo == ingreso.Consecutivo);

    if (existente == null)
    {
        return false;
    }

    bool existeAutomovil = await _context.PrqAutomovils
        .AnyAsync(a => a.Id == ingreso.IdAutomovil);

    bool existeParqueo = await _context.PrqParqueos
        .AnyAsync(p => p.Id == ingreso.IdParqueo);

    if (!existeAutomovil || !existeParqueo)
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

    await _context.SaveChangesAsync();

    return true;
}

public async Task<bool> EliminarAsync(int consecutivo)
{
    var existente = await _context.PrqIngresoAutomovils
        .FirstOrDefaultAsync(i => i.Consecutivo == consecutivo);

    if (existente == null)
    {
        return false;
    }

    _context.PrqIngresoAutomovils.Remove(existente);
    await _context.SaveChangesAsync();

    return true;
}

}
