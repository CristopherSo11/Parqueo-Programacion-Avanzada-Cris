using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Parqueo.BusinessLogic.Repositories.Json;
using Parqueo.BusinessLogic.Repositories.PostgreSQL;
using Parqueo.Model.Context;
using Parqueo.Model.Entities;

Console.WriteLine("=== Prueba de repositorios PostgreSQL ===");

var configuracion = new ConfigurationBuilder()
    .AddUserSecrets("3e9e71a7-627f-49cd-8aea-52a0e4bfb56d")
    .Build();

string? cadenaConexion =
    configuracion.GetConnectionString("ParqueoDB");

if (string.IsNullOrWhiteSpace(cadenaConexion))
{
    Console.WriteLine("No se encontro la cadena de conexion.");
    return;
}

try
{
    var opciones = new DbContextOptionsBuilder<ParqueoDbContext>()
        .UseNpgsql(cadenaConexion)
        .Options;

    await using var contexto = new ParqueoDbContext(opciones);

    // 1. Automoviles
    var automovilRepository =
        new AutomovilPostgresRepository(contexto);

    var automoviles =
        await automovilRepository.ObtenerTodosAsync();

    Console.WriteLine($"\nAutomoviles: {automoviles.Count}");

    foreach (var auto in automoviles)
    {
        Console.WriteLine(
            $"  {auto.Id} - {auto.Fabricante} - " +
            $"{auto.Color} - {auto.Tipo}");
    }

    // 2. Parqueos
    var parqueoRepository =
        new ParqueoPostgresRepository(contexto);

    var parqueos =
        await parqueoRepository.ObtenerTodosAsync();

    Console.WriteLine($"\nParqueos: {parqueos.Count}");

    foreach (var parqueo in parqueos)
    {
        var tarifa =
            await parqueoRepository
                .ObtenerPrecioPorHoraPorParqueo(parqueo.Id);

        Console.WriteLine(
            $"  {parqueo.Id} - {parqueo.Nombre} - " +
            $"{parqueo.NombreProvincia} - " +
            $"Tarifa: {tarifa}");
    }

    // 3. Ingresos
    var ingresoRepository =
        new IngresoPostgresRepository(contexto);

    var ingresos =
        await ingresoRepository.ObtenerTodosAsync();

    Console.WriteLine($"\nIngresos: {ingresos.Count}");

    foreach (var ingreso in ingresos)
    {
        string monto = ingreso.MontoTotal.HasValue
            ? ingreso.MontoTotal.Value.ToString("N2")
            : "Pendiente (sin salida)";

        Console.WriteLine(
            $"  Ingreso {ingreso.Consecutivo} - " +
            $"Entrada: {ingreso.FechaHoraEntrada:g} - " +
            $"Monto: {monto}");
    }

    Console.WriteLine("\nPrueba finalizada correctamente.");
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Error: {ex.GetType().Name}");

    Console.WriteLine(ex.Message);
}

Console.WriteLine("\n=== Prueba de repositorio JSON ===");

try
{
    string rutaJson = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Datos",
        "automoviles.json"
    );

    var repositorioJson =
        new AutomovilJsonRepository(rutaJson);

    var automovilesJson =
        await repositorioJson.ObtenerTodosAsync();

    Console.WriteLine(
        $"Automoviles desde JSON: {automovilesJson.Count}");

    foreach (var auto in automovilesJson)
    {
        Console.WriteLine(
            $"  {auto.Id} - {auto.Fabricante} - " +
            $"{auto.Color} - {auto.Tipo}");
    }

    var filtrados = await repositorioJson.FiltrarAsync(
        fabricante: "Toyota"
    );

    Console.WriteLine(
        $"\nAutomoviles Toyota encontrados: {filtrados.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error JSON: {ex.Message}");
}

Console.WriteLine("\n=== Prueba de parqueos JSON ===");

try
{
    string rutaParqueos = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Datos",
        "parqueos.json"
    );

    var parqueoJsonRepository =
        new ParqueoJsonRepository(rutaParqueos);

    var parqueosJson =
        await parqueoJsonRepository.ObtenerTodosAsync();

    Console.WriteLine($"Parqueos desde JSON: {parqueosJson.Count}");

    foreach (var parqueo in parqueosJson)
    {
        Console.WriteLine(
            $"  {parqueo.Id} - {parqueo.Nombre} - " +
            $"{parqueo.NombreProvincia} - " +
            $"Tarifa: {parqueo.PrecioPorHora:N2}");
    }

    var parqueosCartago =
        await parqueoJsonRepository.FiltrarAsync(
            provincia: "Cartago");

    Console.WriteLine(
        $"\nParqueos en Cartago: {parqueosCartago.Count}");

    var tarifa =
        await parqueoJsonRepository
            .ObtenerPrecioPorHoraPorParqueo(1);

    Console.WriteLine($"Tarifa del parqueo 1: {tarifa:N2}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error JSON parqueos: {ex.Message}");
}

Console.WriteLine("\n=== Prueba de ingresos JSON ===");

try
{
    string carpetaDatos = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Datos"
    );

    var automovilJsonRepo = new AutomovilJsonRepository(
        Path.Combine(carpetaDatos, "automoviles.json")
    );

    var parqueoJsonRepo = new ParqueoJsonRepository(
        Path.Combine(carpetaDatos, "parqueos.json")
    );

    var ingresoJsonRepo = new IngresoJsonRepository(
        Path.Combine(carpetaDatos, "ingresos-automoviles.json"),
        automovilJsonRepo,
        parqueoJsonRepo
    );

    var ingresosJson = await ingresoJsonRepo.ObtenerTodosAsync();

    Console.WriteLine(
        $"Ingresos desde JSON: {ingresosJson.Count}");

    foreach (var ingreso in ingresosJson)
    {
        string monto = ingreso.MontoTotal.HasValue
            ? ingreso.MontoTotal.Value.ToString("N2")
            : "Pendiente (sin salida)";

        Console.WriteLine(
            $"  Ingreso {ingreso.Consecutivo} - " +
            $"Entrada: {ingreso.FechaHoraEntrada:g} - " +
            $"Monto: {monto}");
    }

    var pendientes = ingresosJson.Count(
        i => !i.FechaHoraSalida.HasValue);

    Console.WriteLine(
        $"\nIngresos pendientes de salida: {pendientes}");

    var ingresosCartago =
        await ingresoJsonRepo.FiltrarPorProvinciaYFechasAsync(
            "Cartago", null, null);

    Console.WriteLine(
        $"Ingresos en Cartago: {ingresosCartago.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error JSON ingresos: {ex.Message}");
}

Console.WriteLine("\n=== Prueba CRUD de automoviles PostgreSQL ===");

try
{
    var opcionesCrud = new DbContextOptionsBuilder<ParqueoDbContext>()
        .UseNpgsql(cadenaConexion)
        .Options;

    await using var contextoCrud =
        new ParqueoDbContext(opcionesCrud);

    var repositorioCrud =
        new AutomovilPostgresRepository(contextoCrud);

    // 1. Insertar un automovil temporal.
    var automovilPrueba = new PrqAutomovil
    {
        Color = "Verde",
        Anio = 2024,
        Fabricante = "Prueba CRUD",
        Tipo = "sedán"
    };

    var insertado =
        await repositorioCrud.InsertarAsync(automovilPrueba);

    Console.WriteLine(
        $"Insertado: ID {insertado.Id} - {insertado.Fabricante}");

    // 2. Actualizar el automovil temporal.
    insertado.Color = "Azul";

    bool actualizado =
        await repositorioCrud.ActualizarAsync(insertado);

    Console.WriteLine($"Actualizado: {actualizado}");

    // 3. Consultar para verificar el cambio.
    var consultado =
        await repositorioCrud.ObtenerPorIdAsync(insertado.Id);

    Console.WriteLine(
        $"Color despues de actualizar: {consultado?.Color}");

    // 4. Eliminar el automovil temporal.
    bool eliminado =
        await repositorioCrud.EliminarAsync(insertado.Id);

    Console.WriteLine($"Eliminado: {eliminado}");

    // 5. Verificar que volvimos a los registros originales.
    var automovilesFinales =
        await repositorioCrud.ObtenerTodosAsync();

    Console.WriteLine(
        $"Automoviles restantes: {automovilesFinales.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error CRUD: {ex.GetType().Name}");
    Console.WriteLine(ex.Message);
}

Console.WriteLine("\n=== Prueba CRUD de parqueos PostgreSQL ===");

try
{
    var opcionesParqueoCrud =
        new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseNpgsql(cadenaConexion)
            .Options;

    await using var contextoParqueoCrud =
        new ParqueoDbContext(opcionesParqueoCrud);

    var repositorioParqueoCrud =
        new ParqueoPostgresRepository(contextoParqueoCrud);

    // 1. Insertar un parqueo temporal.
    var parqueoPrueba = new PrqParqueo
    {
        NombreProvincia = "Cartago",
        Nombre = "Parqueo Prueba CRUD",
        PrecioPorHora = 1200m
    };

    var insertado =
        await repositorioParqueoCrud.InsertarAsync(parqueoPrueba);

    Console.WriteLine(
        $"Insertado: ID {insertado.Id} - {insertado.Nombre}");

    // 2. Actualizar la tarifa del parqueo.
    insertado.PrecioPorHora = 1800m;

    bool actualizado =
        await repositorioParqueoCrud.ActualizarAsync(insertado);

    Console.WriteLine($"Actualizado: {actualizado}");

    // 3. Consultar para verificar la nueva tarifa.
    var consultado =
        await repositorioParqueoCrud.ObtenerPorIdAsync(insertado.Id);

    Console.WriteLine(
        $"Tarifa despues de actualizar: {consultado?.PrecioPorHora:N2}");

    // 4. Eliminar el parqueo temporal.
    bool eliminado =
        await repositorioParqueoCrud.EliminarAsync(insertado.Id);

    Console.WriteLine($"Eliminado: {eliminado}");

    // 5. Comprobar los registros restantes.
    var parqueosFinales =
        await repositorioParqueoCrud.ObtenerTodosAsync();

    Console.WriteLine(
        $"Parqueos restantes: {parqueosFinales.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error CRUD parqueos: {ex.GetType().Name}");
    Console.WriteLine(ex.Message);
}

Console.WriteLine("\n=== Prueba CRUD de ingresos PostgreSQL ===");

try
{
    var opcionesIngresoCrud =
        new DbContextOptionsBuilder<ParqueoDbContext>()
            .UseNpgsql(cadenaConexion)
            .Options;

    await using var contextoIngresoCrud =
        new ParqueoDbContext(opcionesIngresoCrud);

    var repositorioIngresoCrud =
        new IngresoPostgresRepository(contextoIngresoCrud);

    // 1. Insertar un ingreso temporal.
    var ingresoPrueba = new PrqIngresoAutomovil
    {
        IdAutomovil = 1,
        IdParqueo = 1,
        FechaHoraEntrada = new DateTime(2026, 10, 15, 8, 0, 0),
        FechaHoraSalida = null
    };

    var insertado =
        await repositorioIngresoCrud.InsertarAsync(ingresoPrueba);

    Console.WriteLine(
        $"Insertado: consecutivo {insertado.Consecutivo}");

    // 2. Registrar la salida del automóvil.
    var ingresoActualizado = new PrqIngresoAutomovil
    {
        Consecutivo = insertado.Consecutivo,
        IdAutomovil = insertado.IdAutomovil,
        IdParqueo = insertado.IdParqueo,
        FechaHoraEntrada = insertado.FechaHoraEntrada,
        FechaHoraSalida = new DateTime(2026, 10, 15, 10, 30, 0)
    };

    bool actualizado =
        await repositorioIngresoCrud.ActualizarAsync(ingresoActualizado);

    Console.WriteLine($"Actualizado: {actualizado}");

    // 3. Consultar el ingreso y calcular su monto.
    var consultado =
        await repositorioIngresoCrud.ObtenerPorIdAsync(
            insertado.Consecutivo);

    Console.WriteLine(
        $"Duracion: {consultado?.DuracionMinutos} minutos");

    Console.WriteLine(
        $"Monto: {consultado?.MontoTotal:N2}");

    // 4. Eliminar el ingreso temporal.
    bool eliminado =
        await repositorioIngresoCrud.EliminarAsync(
            insertado.Consecutivo);

    Console.WriteLine($"Eliminado: {eliminado}");

    // 5. Verificar que permanecen los 15 originales.
    var ingresosFinales =
        await repositorioIngresoCrud.ObtenerTodosAsync();

    Console.WriteLine(
        $"Ingresos restantes: {ingresosFinales.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error CRUD ingresos: {ex.GetType().Name}");
    Console.WriteLine(ex.Message);
}

Console.WriteLine("\n=== Prueba CRUD de automoviles JSON ===");

string? rutaTemporal = null;

try
{
    string rutaOriginal = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Datos",
        "automoviles.json"
    );

    // Crear una copia temporal para las pruebas.
    rutaTemporal = Path.Combine(
        Path.GetTempPath(),
        $"automoviles-prueba-{Guid.NewGuid()}.json"
    );

    File.Copy(rutaOriginal, rutaTemporal);

    var repositorioCrudJson =
        new AutomovilJsonRepository(rutaTemporal);

    // 1. Insertar un automovil temporal.
    var automovilPruebaJson = new PrqAutomovil
    {
        Color = "Verde",
        Anio = 2024,
        Fabricante = "Prueba JSON",
        Tipo = "sedán"
    };

    var insertadoJson =
        await repositorioCrudJson.InsertarAsync(
            automovilPruebaJson);

    Console.WriteLine(
        $"Insertado: ID {insertadoJson.Id} - " +
        $"{insertadoJson.Fabricante}");

    // 2. Actualizar el color.
    insertadoJson.Color = "Azul";

    bool actualizadoJson =
        await repositorioCrudJson.ActualizarAsync(insertadoJson);

    Console.WriteLine($"Actualizado: {actualizadoJson}");

    // 3. Consultar para verificar el cambio.
    var consultadoJson =
        await repositorioCrudJson.ObtenerPorIdAsync(
            insertadoJson.Id);

    Console.WriteLine(
        $"Color despues de actualizar: {consultadoJson?.Color}");

    // 4. Eliminar el automovil temporal.
    bool eliminadoJson =
        await repositorioCrudJson.EliminarAsync(
            insertadoJson.Id);

    Console.WriteLine($"Eliminado: {eliminadoJson}");

    // 5. Comprobar que quedan cinco automoviles.
    var automovilesFinalesJson =
        await repositorioCrudJson.ObtenerTodosAsync();

    Console.WriteLine(
        $"Automoviles restantes: {automovilesFinalesJson.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error CRUD JSON: {ex.GetType().Name}");
    Console.WriteLine(ex.Message);
}
finally
{
    // Eliminar la copia temporal, incluso si ocurre un error.
    if (rutaTemporal != null && File.Exists(rutaTemporal))
    {
        File.Delete(rutaTemporal);
    }
}

Console.WriteLine("\n=== Prueba CRUD de parqueos JSON ===");

string? rutaParqueosTemporal = null;

try
{
    string rutaParqueosOriginal = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Datos",
        "parqueos.json"
    );

    // 1. Crear una copia temporal del archivo original.
    rutaParqueosTemporal = Path.Combine(
        Path.GetTempPath(),
        $"parqueos-prueba-{Guid.NewGuid()}.json"
    );

    File.Copy(rutaParqueosOriginal, rutaParqueosTemporal);

    var repositorioParqueosJsonCrud =
        new ParqueoJsonRepository(rutaParqueosTemporal);

    // 2. Insertar un parqueo temporal.
    var parqueoPruebaJson = new PrqParqueo
    {
        NombreProvincia = "Cartago",
        Nombre = "Parqueo Prueba JSON",
        PrecioPorHora = 1200m
    };

    var insertadoParqueoJson =
        await repositorioParqueosJsonCrud.InsertarAsync(
            parqueoPruebaJson);

    Console.WriteLine(
        $"Insertado: ID {insertadoParqueoJson.Id} - " +
        $"{insertadoParqueoJson.Nombre}");

    // 3. Actualizar la tarifa.
    insertadoParqueoJson.PrecioPorHora = 1800m;

    bool actualizadoParqueoJson =
        await repositorioParqueosJsonCrud.ActualizarAsync(
            insertadoParqueoJson);

    Console.WriteLine(
        $"Actualizado: {actualizadoParqueoJson}");

    // 4. Consultar para verificar el cambio.
    var consultadoParqueoJson =
        await repositorioParqueosJsonCrud.ObtenerPorIdAsync(
            insertadoParqueoJson.Id);

    Console.WriteLine(
        $"Tarifa despues de actualizar: " +
        $"{consultadoParqueoJson?.PrecioPorHora:N2}");

    // 5. Eliminar el parqueo temporal.
    bool eliminadoParqueoJson =
        await repositorioParqueosJsonCrud.EliminarAsync(
            insertadoParqueoJson.Id);

    Console.WriteLine(
        $"Eliminado: {eliminadoParqueoJson}");

    // 6. Verificar que permanecen los dos originales.
    var parqueosFinalesJson =
        await repositorioParqueosJsonCrud.ObtenerTodosAsync();

    Console.WriteLine(
        $"Parqueos restantes: {parqueosFinalesJson.Count}");
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Error CRUD parqueos JSON: {ex.GetType().Name}");

    Console.WriteLine(ex.Message);
}
finally
{
    // Borrar únicamente la copia temporal.
    if (rutaParqueosTemporal != null &&
        File.Exists(rutaParqueosTemporal))
    {
        File.Delete(rutaParqueosTemporal);
    }
}

Console.WriteLine("\n=== Prueba CRUD de ingresos JSON ===");

string? rutaIngresosTemporal = null;

try
{
    string carpetaDatos = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Datos"
    );

    string rutaIngresosOriginal = Path.Combine(
        carpetaDatos,
        "ingresos-automoviles.json"
    );

    // 1. Crear una copia temporal del JSON de ingresos.
    rutaIngresosTemporal = Path.Combine(
        Path.GetTempPath(),
        $"ingresos-prueba-{Guid.NewGuid()}.json"
    );

    File.Copy(rutaIngresosOriginal, rutaIngresosTemporal);

    // 2. Crear los repositorios relacionados.
    var autosRepo = new AutomovilJsonRepository(
        Path.Combine(carpetaDatos, "automoviles.json")
    );

    var parqueosRepo = new ParqueoJsonRepository(
        Path.Combine(carpetaDatos, "parqueos.json")
    );

    var ingresosRepo = new IngresoJsonRepository(
        rutaIngresosTemporal,
        autosRepo,
        parqueosRepo
    );

    // 3. Insertar un ingreso temporal.
    var ingresoPrueba = new PrqIngresoAutomovil
    {
        IdAutomovil = 1,
        IdParqueo = 1,
        FechaHoraEntrada = new DateTime(2026, 10, 15, 8, 0, 0),
        FechaHoraSalida = null
    };

    var insertado = await ingresosRepo.InsertarAsync(ingresoPrueba);

    Console.WriteLine(
        $"Insertado: consecutivo {insertado.Consecutivo}");

    // 4. Registrar la salida.
    var ingresoActualizado = new PrqIngresoAutomovil
    {
        Consecutivo = insertado.Consecutivo,
        IdAutomovil = insertado.IdAutomovil,
        IdParqueo = insertado.IdParqueo,
        FechaHoraEntrada = insertado.FechaHoraEntrada,
        FechaHoraSalida = new DateTime(2026, 10, 15, 10, 30, 0)
    };

    bool actualizado =
        await ingresosRepo.ActualizarAsync(ingresoActualizado);

    Console.WriteLine($"Actualizado: {actualizado}");

    // 5. Consultar el ingreso y comprobar sus cálculos.
    var consultado = await ingresosRepo.ObtenerPorIdAsync(
        insertado.Consecutivo
    );

    Console.WriteLine(
        $"Duracion: {consultado?.DuracionMinutos} minutos");

    Console.WriteLine(
        $"Monto: {consultado?.MontoTotal:N2}");

    // 6. Eliminar el ingreso temporal.
    bool eliminado = await ingresosRepo.EliminarAsync(
        insertado.Consecutivo
    );

    Console.WriteLine($"Eliminado: {eliminado}");

    // 7. Verificar que quedan los 15 originales.
    var ingresosFinales = await ingresosRepo.ObtenerTodosAsync();

    Console.WriteLine(
        $"Ingresos restantes: {ingresosFinales.Count}");
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Error CRUD ingresos JSON: {ex.GetType().Name}");

    Console.WriteLine(ex.Message);
}
finally
{
    if (rutaIngresosTemporal != null &&
        File.Exists(rutaIngresosTemporal))
    {
        File.Delete(rutaIngresosTemporal);
    }
}