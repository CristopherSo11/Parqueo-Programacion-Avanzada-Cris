using Microsoft.EntityFrameworkCore;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Context;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Repositories.PostgreSQL;

public class ParqueoPostgresRepository : IParqueoRepository
{
    private readonly ParqueoDbContext _context;

    public ParqueoPostgresRepository(ParqueoDbContext context)
    {
        _context = context;
    }

    public async Task<List<PrqParqueo>> ObtenerTodosAsync()
    {
        return await _context.PrqParqueos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PrqParqueo?> ObtenerPorIdAsync(int id)
    {
        return await _context.PrqParqueos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<PrqParqueo>> FiltrarAsync(
        string? provincia = null,
        string? nombre = null,
        decimal? precioMinimo = null,
        decimal? precioMaximo = null)
    {
        var consulta = _context.PrqParqueos
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(provincia))
        {
            consulta = consulta.Where(p =>
                EF.Functions.ILike(p.NombreProvincia, $"%{provincia}%"));
        }

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            consulta = consulta.Where(p =>
                EF.Functions.ILike(p.Nombre, $"%{nombre}%"));
        }

        if (precioMinimo.HasValue)
        {
            consulta = consulta.Where(p =>
                p.PrecioPorHora >= precioMinimo.Value);
        }

        if (precioMaximo.HasValue)
        {
            consulta = consulta.Where(p =>
                p.PrecioPorHora <= precioMaximo.Value);
        }

        return await consulta.ToListAsync();
    }

    public async Task<decimal?> ObtenerPrecioPorHoraPorParqueo(int id)
    {
        return await _context.PrqParqueos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => (decimal?)p.PrecioPorHora)
            .FirstOrDefaultAsync();
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
