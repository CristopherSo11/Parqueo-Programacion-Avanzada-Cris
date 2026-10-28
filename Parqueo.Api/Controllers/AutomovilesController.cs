using Microsoft.AspNetCore.Mvc;
using Parqueo.BusinessLogic.Interfaces;
using Parqueo.Model.Entities;

namespace Parqueo.Api.Controllers;

[ApiController]
[Route("api/automoviles")]
public class AutomovilesController : ControllerBase
{
    private readonly IAutomovilRepository _repositorio;

    public AutomovilesController(IAutomovilRepository repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: api/automoviles
    [HttpGet]
    public async Task<ActionResult<List<PrqAutomovil>>> ObtenerTodos()
    {
        var automoviles = await _repositorio.ObtenerTodosAsync();
        return Ok(automoviles);
    }

    // GET: api/automoviles/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrqAutomovil>> ObtenerPorId(int id)
    {
        var automovil = await _repositorio.ObtenerPorIdAsync(id);

        if (automovil == null)
            return NotFound(new { mensaje = "Automóvil no encontrado." });

        return Ok(automovil);
    }

    // GET: api/automoviles/filtrar?color=rojo
    [HttpGet("filtrar")]
    public async Task<ActionResult<List<PrqAutomovil>>> Filtrar(
        [FromQuery] string? color,
        [FromQuery] int? anioDesde,
        [FromQuery] int? anioHasta,
        [FromQuery] string? fabricante,
        [FromQuery] string? tipo)
    {
        var automoviles = await _repositorio.FiltrarAsync(
            color, anioDesde, anioHasta, fabricante, tipo);

        return Ok(automoviles);
    }
    
    // POST: api/automoviles
    [HttpPost]
    public async Task<ActionResult<PrqAutomovil>> Crear(
        [FromBody] PrqAutomovil automovil)
    {
        var creado = await _repositorio.InsertarAsync(automovil);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = creado.Id },
            creado);
    }

    // PUT: api/automoviles/100
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] PrqAutomovil automovil)
    {
        if (id != automovil.Id)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el automóvil."
            });
        }

        var actualizado = await _repositorio.ActualizarAsync(automovil);

        if (!actualizado)
        {
            return NotFound(new
            {
                mensaje = "Automóvil no encontrado."
            });
        }

        return NoContent();
    }

    // DELETE: api/automoviles/100
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _repositorio.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = "Automóvil no encontrado."
            });
        }

        return NoContent();
    }

}
