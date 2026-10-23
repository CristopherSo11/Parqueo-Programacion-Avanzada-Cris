using System;
using Parqueo.Model.Entities;
using Xunit;

namespace Parqueo.Tests;

public class CalculosIngresoTests
{
    [Fact]
    public void CalcularDuracionYMonto_ConSalida()
    {
        // Arrange
        var parqueo = new PrqParqueo
        {
            Id = 1,
            Nombre = "Parqueo de prueba",
            NombreProvincia = "Cartago",
            PrecioPorHora = 1000m
        };

        var ingreso = new PrqIngresoAutomovil
        {
            Consecutivo = 1,
            IdParqueo = 1,
            IdAutomovil = 1,
            FechaHoraEntrada = new DateTime(2026, 10, 22, 8, 0, 0),
            FechaHoraSalida = new DateTime(2026, 10, 22, 10, 30, 0),
            IdParqueoNavigation = parqueo
        };

        // Assert
        Assert.Equal(150, ingreso.DuracionMinutos);
        Assert.Equal(2.5, ingreso.DuracionHoras);
        Assert.Equal(2500m, ingreso.MontoTotal);
    }

    [Fact]
    public void CalcularDuracionYMonto_SinSalida()
    {
        // Arrange
        var ingreso = new PrqIngresoAutomovil
        {
            Consecutivo = 2,
            IdParqueo = 1,
            IdAutomovil = 1,
            FechaHoraEntrada = new DateTime(2026, 10, 22, 8, 0, 0),
            FechaHoraSalida = null
        };

        // Assert
        Assert.Null(ingreso.DuracionMinutos);
        Assert.Null(ingreso.DuracionHoras);
        Assert.Null(ingreso.MontoTotal);
    }
}
