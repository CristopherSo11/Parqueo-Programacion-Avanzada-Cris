using Parqueo.BusinessLogic.Repositories.Json;
using Parqueo.Model.Entities;

namespace Parqueo.Tests;

public class IngresoJsonRepositoryTests
{

[Fact]
public async Task ActualizarAsync_IngresoInexistente_RetornaFalse()
{
    string carpetaDatos = Path.Combine(
        BuscarRaizProyecto(),
        "Datos"
    );

    string rutaTemporal = Path.Combine(
        Path.GetTempPath(),
        $"ingresos-actualizar-test-{Guid.NewGuid()}.json"
    );

    File.Copy(
        Path.Combine(carpetaDatos, "ingresos-automoviles.json"),
        rutaTemporal
    );

    try
    {
        var autos = new AutomovilJsonRepository(
            Path.Combine(carpetaDatos, "automoviles.json")
        );

        var parqueos = new ParqueoJsonRepository(
            Path.Combine(carpetaDatos, "parqueos.json")
        );

        var repositorio = new IngresoJsonRepository(
            rutaTemporal,
            autos,
            parqueos
        );

        int cantidadAntes =
            (await repositorio.ObtenerTodosAsync()).Count;

        var ingresoInexistente = new PrqIngresoAutomovil
        {
            Consecutivo = 99999,
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = new DateTime(2026, 10, 15, 8, 0, 0),
            FechaHoraSalida = new DateTime(2026, 10, 15, 10, 0, 0)
        };

        bool actualizado =
            await repositorio.ActualizarAsync(ingresoInexistente);

        int cantidadDespues =
            (await repositorio.ObtenerTodosAsync()).Count;

        Assert.False(actualizado);
        Assert.Equal(cantidadAntes, cantidadDespues);
    }
    finally
    {
        if (File.Exists(rutaTemporal))
        {
            File.Delete(rutaTemporal);
        }
    }
}

[Fact]
public async Task EliminarAsync_IngresoInexistente_RetornaFalse()
{
    string carpetaDatos = Path.Combine(
        BuscarRaizProyecto(),
        "Datos"
    );

    string rutaTemporal = Path.Combine(
        Path.GetTempPath(),
        $"ingresos-eliminar-test-{Guid.NewGuid()}.json"
    );

    File.Copy(
        Path.Combine(carpetaDatos, "ingresos-automoviles.json"),
        rutaTemporal
    );

    try
    {
        var autos = new AutomovilJsonRepository(
            Path.Combine(carpetaDatos, "automoviles.json")
        );

        var parqueos = new ParqueoJsonRepository(
            Path.Combine(carpetaDatos, "parqueos.json")
        );

        var repositorio = new IngresoJsonRepository(
            rutaTemporal,
            autos,
            parqueos
        );

        int cantidadAntes =
            (await repositorio.ObtenerTodosAsync()).Count;

        bool eliminado =
            await repositorio.EliminarAsync(99999);

        int cantidadDespues =
            (await repositorio.ObtenerTodosAsync()).Count;

        Assert.False(eliminado);
        Assert.Equal(cantidadAntes, cantidadDespues);
    }
    finally
    {
        if (File.Exists(rutaTemporal))
        {
            File.Delete(rutaTemporal);
        }
    }
}

[Fact]
public async Task InsertarAsync_SalidaAnteriorEntrada_LanzaExcepcion()
{
    string carpetaDatos = Path.Combine(
        BuscarRaizProyecto(),
        "Datos"
    );

    string rutaTemporal = Path.Combine(
        Path.GetTempPath(),
        $"ingresos-fecha-test-{Guid.NewGuid()}.json"
    );

    File.Copy(
        Path.Combine(carpetaDatos, "ingresos-automoviles.json"),
        rutaTemporal
    );

    try
    {
        var autos = new AutomovilJsonRepository(
            Path.Combine(carpetaDatos, "automoviles.json")
        );

        var parqueos = new ParqueoJsonRepository(
            Path.Combine(carpetaDatos, "parqueos.json")
        );

        var repositorio = new IngresoJsonRepository(
            rutaTemporal,
            autos,
            parqueos
        );

        var ingreso = new PrqIngresoAutomovil
        {
            IdAutomovil = 1,
            IdParqueo = 1,
            FechaHoraEntrada = new DateTime(2026, 10, 15, 10, 0, 0),
            FechaHoraSalida = new DateTime(2026, 10, 15, 8, 0, 0)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => repositorio.InsertarAsync(ingreso)
        );
    }
    finally
    {
        if (File.Exists(rutaTemporal))
        {
            File.Delete(rutaTemporal);
        }
    }
}

    [Fact]
    public async Task InsertarAsync_AutomovilInexistente_LanzaExcepcion()
    {
        // Preparar una copia temporal del archivo.
        string carpetaDatos = Path.Combine(
            BuscarRaizProyecto(),
            "Datos"
        );

        string rutaTemporal = Path.Combine(
            Path.GetTempPath(),
            $"ingresos-test-{Guid.NewGuid()}.json"
        );

        File.Copy(
            Path.Combine(carpetaDatos, "ingresos-automoviles.json"),
            rutaTemporal
        );

        try
        {
            var autos = new AutomovilJsonRepository(
                Path.Combine(carpetaDatos, "automoviles.json")
            );

            var parqueos = new ParqueoJsonRepository(
                Path.Combine(carpetaDatos, "parqueos.json")
            );

            var repositorio = new IngresoJsonRepository(
                rutaTemporal,
                autos,
                parqueos
            );

            var ingreso = new PrqIngresoAutomovil
            {
                IdAutomovil = 99999,
                IdParqueo = 1,
                FechaHoraEntrada = new DateTime(2026, 10, 15, 8, 0, 0)
            };

            await Assert.ThrowsAsync<ArgumentException>(
                () => repositorio.InsertarAsync(ingreso)
            );
        }
        finally
        {
            if (File.Exists(rutaTemporal))
                File.Delete(rutaTemporal);
        }
    }

    private static string BuscarRaizProyecto()
    {
        var directorio = new DirectoryInfo(
            AppContext.BaseDirectory
        );

        while (directorio != null)
        {
            if (File.Exists(
                Path.Combine(directorio.FullName, "Parqueo.sln")))
            {
                return directorio.FullName;
            }

            directorio = directorio.Parent;
        }

        throw new DirectoryNotFoundException(
            "No se encontro la raiz del proyecto."
        );
    }
}
