using System.Collections.Generic;
using System.Threading.Tasks;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Interfaces;

public interface IParqueoRepository
{
    Task<List<PrqParqueo>> ObtenerTodosAsync();

    Task<PrqParqueo?> ObtenerPorIdAsync(int id);

    Task<List<PrqParqueo>> FiltrarAsync(
        string? provincia = null,
        string? nombre = null,
        decimal? precioMinimo = null,
        decimal? precioMaximo = null
    );

    Task<decimal?> ObtenerPrecioPorHoraPorParqueo(int id);

    Task<PrqParqueo> InsertarAsync(PrqParqueo parqueo);

    Task<bool> ActualizarAsync(PrqParqueo parqueo);

    Task<bool> EliminarAsync(int id);
}
