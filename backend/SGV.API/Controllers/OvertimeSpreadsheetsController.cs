using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.OvertimeSpreadsheet;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OvertimeSpreadsheetsController : ControllerBase
{
    private readonly IOvertimeSpreadsheetService _service;

    public OvertimeSpreadsheetsController(IOvertimeSpreadsheetService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OvertimeSpreadsheetSummaryDTO>>> GetAll(
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] string? search)
        => Ok(await _service.GetByFilterAsync(month, year, search));

    [HttpGet("{id}")]
    public async Task<ActionResult<OvertimeSpreadsheetDTO>> GetById(int id)
    {
        var spreadsheet = await _service.GetByIdAsync(id);
        return spreadsheet == null
            ? NotFound(new { Message = $"No se encontró la planilla con Id {id}." })
            : Ok(spreadsheet);
    }

    [HttpPost]
    public async Task<ActionResult<OvertimeSpreadsheetDTO>> Create([FromBody] OvertimeSpreadsheetCreateDTO dto)
    {
        try
        {
            var spreadsheet = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = spreadsheet.Id }, spreadsheet);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
