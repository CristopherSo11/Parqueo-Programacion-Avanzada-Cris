using Microsoft.Extensions.Configuration;
using Npgsql;

Console.WriteLine("=== Verificacion de registros PRQ ===");

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
    await using var conexion = new NpgsqlConnection(cadenaConexion);
    await conexion.OpenAsync();

    string? esquema;

    await using (var verificar = new NpgsqlCommand(
        "SELECT current_schema()", conexion))
    {
        esquema = (string?)await verificar.ExecuteScalarAsync();
    }

    if (esquema != "user_12")
    {
        Console.WriteLine($"Esquema inesperado: {esquema}");
        return;
    }

    const string sql = """
        SELECT
            (SELECT COUNT(*) FROM user_12.prq_automovil),
            (SELECT COUNT(*) FROM user_12.prq_parqueo),
            (SELECT COUNT(*) FROM user_12.prq_ingreso_automovil),
            (SELECT COUNT(*)
             FROM user_12.prq_ingreso_automovil
             WHERE fecha_hora_salida IS NULL);
        """;

    await using var comando = new NpgsqlCommand(sql, conexion);
    await using var lector = await comando.ExecuteReaderAsync();

    if (await lector.ReadAsync())
    {
        Console.WriteLine($"Automoviles: {lector.GetInt64(0)}");
        Console.WriteLine($"Parqueos: {lector.GetInt64(1)}");
        Console.WriteLine($"Ingresos: {lector.GetInt64(2)}");
        Console.WriteLine($"Sin salida: {lector.GetInt64(3)}");
    }
}
catch (PostgresException ex)
{
    Console.WriteLine($"Error PostgreSQL: {ex.SqlState}");
    Console.WriteLine(ex.MessageText);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.GetType().Name}");
    Console.WriteLine(ex.Message);
}
