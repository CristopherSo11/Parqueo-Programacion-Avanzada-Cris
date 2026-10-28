
using Microsoft.AspNetCore.Mvc;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Entities;

namespace Parqueo.Api.Controllers;

[ApiController]
[Route("api/parqueos")]
public class ParqueosController : ControllerBase
{
    private readonly IParqueoRepository _repositorio;

    public ParqueosController(IParqueoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    [HttpGet]
    public async Task<ActionResult<List<PrqParqueo>>> ObtenerTodos()
    {
        var parqueos = await _repositorio.ObtenerTodosAsync();
        return Ok(parqueos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrqParqueo>> ObtenerPorId(int id)
    {
        var parqueo = await _repositorio.ObtenerPorIdAsync(id);

        if (parqueo == null)
            return NotFound(new { mensaje = "Parqueo no encontrado." });

        return Ok(parqueo);
    }

    [HttpGet("filtrar")]
    public async Task<ActionResult<List<PrqParqueo>>> Filtrar(
        [FromQuery] string? provincia,
        [FromQuery] string? nombre,
        [FromQuery] decimal? precioMinimo,
        [FromQuery] decimal? precioMaximo)
    {
        var parqueos = await _repositorio.FiltrarAsync(
            provincia, nombre, precioMinimo, precioMaximo);

        return Ok(parqueos);
    }

    [HttpGet("{id:int}/precio")]
    public async Task<IActionResult> ObtenerPrecio(int id)
    {
        var precio = await _repositorio.ObtenerPrecioPorHoraPorParqueo(id);

        if (precio == null)
            return NotFound(new { mensaje = "Parqueo no encontrado." });

        return Ok(new { idParqueo = id, precioPorHora = precio });
    }

    [HttpPost]
    public async Task<ActionResult<PrqParqueo>> Crear(
        [FromBody] PrqParqueo parqueo)
    {
        var creado = await _repositorio.InsertarAsync(parqueo);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = creado.Id },
            creado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] PrqParqueo parqueo)
    {
        if (id != parqueo.Id)
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el parqueo."
            });

        var actualizado = await _repositorio.ActualizarAsync(parqueo);

        if (!actualizado)
            return NotFound(new { mensaje = "Parqueo no encontrado." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _repositorio.EliminarAsync(id);

        if (!eliminado)
            return NotFound(new { mensaje = "Parqueo no encontrado." });

        return NoContent();
    }
}
