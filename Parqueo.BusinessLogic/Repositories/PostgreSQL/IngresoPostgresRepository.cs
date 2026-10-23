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
