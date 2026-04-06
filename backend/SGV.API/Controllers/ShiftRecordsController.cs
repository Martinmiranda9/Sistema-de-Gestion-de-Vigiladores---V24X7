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

    [HttpGet("guard/{securityGuardId}")]
    public async Task<ActionResult<IEnumerable<ShiftRecordDTO>>> GetBySecurityGuard(
        int securityGuardId, [FromQuery] int month, [FromQuery] int year)
        => Ok(await _service.GetBySecurityGuardAsync(securityGuardId, month, year));
}
