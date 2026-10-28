
using Microsoft.AspNetCore.Mvc;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Entities;

namespace Parqueo.Api.Controllers;

[ApiController]
[Route("api/ingresos")]
public class IngresosAutomovilesController : ControllerBase
{
    private readonly IIngresoAutomovilRepository _repositorio;

    public IngresosAutomovilesController(
        IIngresoAutomovilRepository repositorio)
    {
        _repositorio = repositorio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var ingresos = await _repositorio.ObtenerTodosAsync();
        return Ok(ingresos);
    }

    [HttpGet("{consecutivo:int}")]
    public async Task<IActionResult> ObtenerPorId(int consecutivo)
    {
        var ingreso = await _repositorio.ObtenerPorIdAsync(consecutivo);

        if (ingreso == null)
            return NotFound(new { mensaje = "Ingreso no encontrado." });

        return Ok(ingreso);
    }

    [HttpGet("filtrar/tipo")]
    public async Task<IActionResult> FiltrarPorTipo(
        [FromQuery] string? tipo,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        var ingresos = await _repositorio.FiltrarPorTipoYFechasAsync(
            tipo, fechaDesde, fechaHasta);

        return Ok(ingresos);
    }

    [HttpGet("filtrar/provincia")]
    public async Task<IActionResult> FiltrarPorProvincia(
        [FromQuery] string? provincia,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
    {
        var ingresos = await _repositorio.FiltrarPorProvinciaYFechasAsync(
            provincia, fechaDesde, fechaHasta);

        return Ok(ingresos);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] PrqIngresoAutomovil ingreso)
    {
        try
        {
            var creado = await _repositorio.InsertarAsync(ingreso);

            var resultado = await _repositorio.ObtenerPorIdAsync(
                creado.Consecutivo);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { consecutivo = creado.Consecutivo },
                resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{consecutivo:int}")]
    public async Task<IActionResult> Actualizar(
        int consecutivo,
        [FromBody] PrqIngresoAutomovil ingreso)
    {
        if (consecutivo != ingreso.Consecutivo)
            return BadRequest(new
            {
                mensaje = "El consecutivo no coincide."
            });

        try
        {
            var actualizado = await _repositorio.ActualizarAsync(ingreso);

            if (!actualizado)
                return NotFound(new { mensaje = "Ingreso no encontrado." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{consecutivo:int}")]
    public async Task<IActionResult> Eliminar(int consecutivo)
    {
        var eliminado = await _repositorio.EliminarAsync(consecutivo);

        if (!eliminado)
            return NotFound(new { mensaje = "Ingreso no encontrado." });

        return NoContent();
    }
}
