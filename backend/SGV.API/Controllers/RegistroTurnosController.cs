using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.RegistroTurno;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistroTurnosController : ControllerBase
{
    private readonly IRegistroTurnoService _registroTurnoService;

    public RegistroTurnosController(IRegistroTurnoService registroTurnoService)
    {
        _registroTurnoService = registroTurnoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistroTurnoDTO>>> GetAll()
    {
        var registros = await _registroTurnoService.GetAllAsync();
        return Ok(registros);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegistroTurnoDTO>> GetById(int id)
    {
        var registro = await _registroTurnoService.GetByIdAsync(id);
        if (registro == null)
            return NotFound(new { Message = $"No se encontró el registro de turno con Id {id}." });

        return Ok(registro);
    }

    [HttpPost]
    public async Task<ActionResult<RegistroTurnoDTO>> Create([FromBody] RegistroTurnoCreateDTO dto)
    {
        var registro = await _registroTurnoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = registro.Id }, registro);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RegistroTurnoDTO>> Update(int id, [FromBody] RegistroTurnoUpdateDTO dto)
    {
        var registro = await _registroTurnoService.UpdateAsync(id, dto);
        if (registro == null)
            return NotFound(new { Message = $"No se encontró el registro de turno con Id {id}." });

        return Ok(registro);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _registroTurnoService.DeleteAsync(id);
        if (!result)
            return NotFound(new { Message = $"No se encontró el registro de turno con Id {id}." });

        return NoContent();
    }

    /// <summary>
    /// Obtiene los turnos de un vigilador en un mes/año (para el almanaque).
    /// </summary>
    [HttpGet("vigilador/{vigiladorId}")]
    public async Task<ActionResult<IEnumerable<RegistroTurnoDTO>>> GetByVigilador(
        int vigiladorId, [FromQuery] int mes, [FromQuery] int anio)
    {
        var registros = await _registroTurnoService.GetByVigiladorAsync(vigiladorId, mes, anio);
        return Ok(registros);
    }
}
