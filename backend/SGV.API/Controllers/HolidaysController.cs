using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.Holiday;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService _service;

    public HolidaysController(IHolidayService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<HolidayDTO>> GetById(int id)
    {
        var holiday = await _service.GetByIdAsync(id);
        return holiday == null
            ? NotFound(new { Message = $"No se encontró el feriado con Id {id}." })
            : Ok(holiday);
    }

    [HttpPost]
    public async Task<ActionResult<HolidayDTO>> Create([FromBody] HolidayCreateDTO dto)
    {
        var holiday = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = holiday.Id }, holiday);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HolidayDTO>> Update(int id, [FromBody] HolidayUpdateDTO dto)
    {
        var holiday = await _service.UpdateAsync(id, dto);
        return holiday == null
            ? NotFound(new { Message = $"No se encontró el feriado con Id {id}." })
            : Ok(holiday);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"No se encontró el feriado con Id {id}." });
    }

    [HttpGet("year/{year}")]
    public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetByYear(int year)
        => Ok(await _service.GetByYearAsync(year));

    [HttpGet("check")]
    public async Task<ActionResult> CheckIsHoliday([FromQuery] DateTime date)
    {
        var isHoliday = await _service.IsHolidayAsync(date);
        return Ok(new { Fecha = date.Date, EsFeriado = isHoliday });
    }
}
