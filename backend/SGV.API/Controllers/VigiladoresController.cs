using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.Vigilador;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VigiladoresController : ControllerBase
{
    private readonly IVigiladorService _vigiladorService;

    public VigiladoresController(IVigiladorService vigiladorService)
    {
        _vigiladorService = vigiladorService;
    }

    /// <summary>
    /// Obtiene todos los vigiladores.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VigiladorDTO>>> GetAll()
    {
        var vigiladores = await _vigiladorService.GetAllAsync();
        return Ok(vigiladores);
    }

    /// <summary>
    /// Obtiene un vigilador por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VigiladorDTO>> GetById(int id)
    {
        var vigilador = await _vigiladorService.GetByIdAsync(id);
        if (vigilador == null)
            return NotFound(new { Message = $"No se encontró el vigilador con Id {id}." });

        return Ok(vigilador);
    }

    /// <summary>
    /// Crea un nuevo vigilador.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VigiladorDTO>> Create([FromBody] VigiladorCreateDTO dto)
    {
        var vigilador = await _vigiladorService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = vigilador.Id }, vigilador);
    }

    /// <summary>
    /// Actualiza un vigilador existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<VigiladorDTO>> Update(int id, [FromBody] VigiladorUpdateDTO dto)
    {
        var vigilador = await _vigiladorService.UpdateAsync(id, dto);
        if (vigilador == null)
            return NotFound(new { Message = $"No se encontró el vigilador con Id {id}." });

        return Ok(vigilador);
    }

    /// <summary>
    /// Elimina (baja lógica) un vigilador.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _vigiladorService.DeleteAsync(id);
        if (!result)
            return NotFound(new { Message = $"No se encontró el vigilador con Id {id}." });

        return NoContent();
    }

    /// <summary>
    /// Busca vigiladores por objetivo (lugar asignado).
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<VigiladorDTO>>> GetByObjetivo([FromQuery] int objetivoId)
    {
        var vigiladores = await _vigiladorService.GetByObjetivoAsync(objetivoId);
        return Ok(vigiladores);
    }
}
