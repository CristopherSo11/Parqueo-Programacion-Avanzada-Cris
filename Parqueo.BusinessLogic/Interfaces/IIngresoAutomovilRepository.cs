using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parqueo.Model.Entities;

namespace Parqueo.BusinessLogic.Interfaces;

public interface IIngresoAutomovilRepository
{
    Task<List<PrqIngresoAutomovil>> ObtenerTodosAsync();

    Task<PrqIngresoAutomovil?> ObtenerPorIdAsync(int consecutivo);

    Task<List<PrqIngresoAutomovil>> FiltrarPorTipoYFechasAsync(
        string? tipo,
        DateTime? fechaDesde,
        DateTime? fechaHasta
    );

    Task<List<PrqIngresoAutomovil>> FiltrarPorProvinciaYFechasAsync(
        string? provincia,
        DateTime? fechaDesde,
        DateTime? fechaHasta
    );

    Task<PrqIngresoAutomovil> InsertarAsync(
        PrqIngresoAutomovil ingreso
    );

    Task<bool> ActualizarAsync(
        PrqIngresoAutomovil ingreso
    );

    Task<bool> EliminarAsync(int consecutivo);
}
