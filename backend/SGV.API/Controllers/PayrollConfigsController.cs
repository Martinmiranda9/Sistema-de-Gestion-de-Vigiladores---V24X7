using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.PayrollConfig;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollConfigsController : ControllerBase
{
    private readonly IPayrollConfigService _service;

    public PayrollConfigsController(IPayrollConfigService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PayrollConfigDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<PayrollConfigDTO>> GetById(int id)
    {
        var config = await _service.GetByIdAsync(id);
        return config == null
            ? NotFound(new { Message = $"No se encontró la configuración con Id {id}." })
            : Ok(config);
    }

    [HttpPost]
    public async Task<ActionResult<PayrollConfigDTO>> Create([FromBody] PayrollConfigCreateDTO dto)
    {
        var config = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = config.Id }, config);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PayrollConfigDTO>> Update(int id, [FromBody] PayrollConfigUpdateDTO dto)
    {
        var config = await _service.UpdateAsync(id, dto);
        return config == null
            ? NotFound(new { Message = $"No se encontró la configuración con Id {id}." })
            : Ok(config);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"No se encontró la configuración con Id {id}." });
    }

    [HttpGet("current")]
    public async Task<ActionResult<PayrollConfigDTO>> GetCurrent([FromQuery] DateTime date)
    {
        var config = await _service.GetCurrentAsync(date);
        return config == null
            ? NotFound(new { Message = "No hay configuración vigente para esa fecha." })
            : Ok(config);
    }
}
