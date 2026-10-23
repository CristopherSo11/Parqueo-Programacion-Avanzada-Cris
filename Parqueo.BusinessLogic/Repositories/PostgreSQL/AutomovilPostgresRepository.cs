using Microsoft.EntityFrameworkCore;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Context;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Repositories.PostgreSQL;

public class AutomovilPostgresRepository : IAutomovilRepository
{
    private readonly ParqueoDbContext _context;

    public AutomovilPostgresRepository(ParqueoDbContext context)
    {
        _context = context;
    }

    public async Task<List<PrqAutomovil>> ObtenerTodosAsync()
    {
        return await _context.PrqAutomovils
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PrqAutomovil?> ObtenerPorIdAsync(int id)
    {
        return await _context.PrqAutomovils
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<PrqAutomovil>> FiltrarAsync(
        string? color = null,
        int? anioDesde = null,
        int? anioHasta = null,
        string? fabricante = null,
        string? tipo = null)
    {
        var consulta = _context.PrqAutomovils
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(color))
        {
            consulta = consulta.Where(a =>
                EF.Functions.ILike(a.Color, $"%{color}%"));
        }

        if (anioDesde.HasValue)
        {
            consulta = consulta.Where(a =>
                a.Anio >= anioDesde.Value);
        }

        if (anioHasta.HasValue)
        {
            consulta = consulta.Where(a =>
                a.Anio <= anioHasta.Value);
        }

        if (!string.IsNullOrWhiteSpace(fabricante))
        {
            consulta = consulta.Where(a =>
                EF.Functions.ILike(a.Fabricante, $"%{fabricante}%"));
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            consulta = consulta.Where(a =>
                EF.Functions.ILike(a.Tipo, $"%{tipo}%"));
        }

        return await consulta.ToListAsync();
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
