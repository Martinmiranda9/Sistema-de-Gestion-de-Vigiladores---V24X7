using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.AttendanceSheet;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceSheetsController : ControllerBase
{
    private readonly IAttendanceSheetService _service;

    public AttendanceSheetsController(IAttendanceSheetService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttendanceSheetDTO>>> GetAll(
        [FromQuery] int? workplaceId, 
        [FromQuery] int? securityGuardId, 
        [FromQuery] int? month, 
        [FromQuery] int? year)
    {
        return Ok(await _service.GetAllAsync(workplaceId, securityGuardId, month, year));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AttendanceSheetDTO>> GetById(int id)
    {
        var sheet = await _service.GetByIdAsync(id);
        if (sheet == null) return NotFound(new { Message = $"No se encontró la planilla de asistencia con Id {id}." });
        return Ok(sheet);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceSheetDTO>> Create([FromBody] AttendanceSheetCreateDTO dto)
    {
        try
        {
            var sheet = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = sheet.Id }, sheet);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound(new { Message = $"No se encontró la planilla con Id {id}." });
        return NoContent();
    }
}
