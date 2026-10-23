using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parqueo.Model.Entities;

public partial class PrqIngresoAutomovil
{
    [NotMapped]
    public double? DuracionMinutos
    {
        get
        {
            if (!FechaHoraSalida.HasValue)
                return null;

            return (FechaHoraSalida.Value - FechaHoraEntrada).TotalMinutes;
        }
    }

    [NotMapped]
    public double? DuracionHoras
    {
        get
        {
            if (!DuracionMinutos.HasValue)
                return null;

            return DuracionMinutos.Value / 60.0;
        }
    }

    [NotMapped]
    public decimal? MontoTotal
    {
        get
        {
            if (!DuracionHoras.HasValue || IdParqueoNavigation == null)
                return null;

            return (decimal)DuracionHoras.Value
                * IdParqueoNavigation.PrecioPorHora;
        }
    }
}
