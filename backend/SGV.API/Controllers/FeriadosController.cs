using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.Feriado;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeriadosController : ControllerBase
{
    private readonly IFeriadoService _feriadoService;

    public FeriadosController(IFeriadoService feriadoService)
    {
        _feriadoService = feriadoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeriadoDTO>>> GetAll()
    {
        var feriados = await _feriadoService.GetAllAsync();
        return Ok(feriados);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FeriadoDTO>> GetById(int id)
    {
        var feriado = await _feriadoService.GetByIdAsync(id);
        if (feriado == null)
            return NotFound(new { Message = $"No se encontró el feriado con Id {id}." });

        return Ok(feriado);
    }

    [HttpPost]
    public async Task<ActionResult<FeriadoDTO>> Create([FromBody] FeriadoCreateDTO dto)
    {
        var feriado = await _feriadoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = feriado.Id }, feriado);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FeriadoDTO>> Update(int id, [FromBody] FeriadoUpdateDTO dto)
    {
        var feriado = await _feriadoService.UpdateAsync(id, dto);
        if (feriado == null)
            return NotFound(new { Message = $"No se encontró el feriado con Id {id}." });

        return Ok(feriado);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _feriadoService.DeleteAsync(id);
        if (!result)
            return NotFound(new { Message = $"No se encontró el feriado con Id {id}." });

        return NoContent();
    }

    /// <summary>
    /// Obtiene feriados por año (incluye recurrentes).
    /// </summary>
    [HttpGet("anio/{anio}")]
    public async Task<ActionResult<IEnumerable<FeriadoDTO>>> GetByAnio(int anio)
    {
        var feriados = await _feriadoService.GetByAnioAsync(anio);
        return Ok(feriados);
    }

    /// <summary>
    /// Verifica si una fecha específica es feriado.
    /// </summary>
    [HttpGet("verificar")]
    public async Task<ActionResult<bool>> EsFeriado([FromQuery] DateTime fecha)
    {
        var esFeriado = await _feriadoService.EsFeriadoAsync(fecha);
        return Ok(new { Fecha = fecha.Date, EsFeriado = esFeriado });
    }
}
