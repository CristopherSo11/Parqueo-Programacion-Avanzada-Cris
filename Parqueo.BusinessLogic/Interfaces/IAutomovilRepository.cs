using System.Collections.Generic;
using System.Threading.Tasks;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Interfaces;

public interface IAutomovilRepository
{
    Task<List<PrqAutomovil>> ObtenerTodosAsync();

    Task<PrqAutomovil?> ObtenerPorIdAsync(int id);

    Task<List<PrqAutomovil>> FiltrarAsync(
        string? color = null,
        int? anioDesde = null,
        int? anioHasta = null,
        string? fabricante = null,
        string? tipo = null
    );

    Task<PrqAutomovil> InsertarAsync(PrqAutomovil automovil);

    Task<bool> ActualizarAsync(PrqAutomovil automovil);

    Task<bool> EliminarAsync(int id);
}
