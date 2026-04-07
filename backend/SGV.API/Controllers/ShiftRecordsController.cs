using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.ShiftRecord;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftRecordsController : ControllerBase
{
    private readonly IShiftRecordService _service;

    public ShiftRecordsController(IShiftRecordService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftRecordDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftRecordDTO>> GetById(int id)
    {
        var record = await _service.GetByIdAsync(id);
        return record == null
            ? NotFound(new { Message = $"No se encontró el registro de turno con Id {id}." })
            : Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftRecordDTO>> Create([FromBody] ShiftRecordCreateDTO dto)
    {
        var record = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ShiftRecordDTO>> Update(int id, [FromBody] ShiftRecordUpdateDTO dto)
    {
        var record = await _service.UpdateAsync(id, dto);
        return record == null
            ? NotFound(new { Message = $"No se encontró el registro de turno con Id {id}." })
            : Ok(record);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"No se encontró el registro de turno con Id {id}." });
    }

    /// <summary>
    /// Turnos de un vigilador filtrados por mes y año.
    /// GET /api/shiftrecords/guard/5?month=4&year=2026
    /// </summary>
    [HttpGet("guard/{securityGuardId}")]
    public async Task<ActionResult<IEnumerable<ShiftRecordDTO>>> GetBySecurityGuard(
        int securityGuardId, [FromQuery] int month, [FromQuery] int year)
        => Ok(await _service.GetBySecurityGuardAsync(securityGuardId, month, year));

    /// <summary>
    /// Totalizador mensual: horas totales, nocturnas, feriadas y normales.
    /// GET /api/shiftrecords/summary/5?month=4&year=2026
    /// </summary>
    [HttpGet("summary/{securityGuardId}")]
    public async Task<ActionResult<ShiftSummaryDTO>> GetSummary(
        int securityGuardId, [FromQuery] int month, [FromQuery] int year)
    {
        var summary = await _service.GetSummaryAsync(securityGuardId, month, year);
        return summary == null
            ? NotFound(new { Message = $"No se encontró el vigilador con Id {securityGuardId}." })
            : Ok(summary);
    }
}
