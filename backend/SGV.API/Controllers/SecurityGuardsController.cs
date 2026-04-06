using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.SecurityGuard;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecurityGuardsController : ControllerBase
{
    private readonly ISecurityGuardService _service;

    public SecurityGuardsController(ISecurityGuardService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SecurityGuardDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<SecurityGuardDTO>> GetById(int id)
    {
        var guard = await _service.GetByIdAsync(id);
        return guard == null
            ? NotFound(new { Message = $"No se encontró el vigilador con Id {id}." })
            : Ok(guard);
    }

    [HttpPost]
    public async Task<ActionResult<SecurityGuardDTO>> Create([FromBody] SecurityGuardCreateDTO dto)
    {
        var guard = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = guard.Id }, guard);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SecurityGuardDTO>> Update(int id, [FromBody] SecurityGuardUpdateDTO dto)
    {
        var guard = await _service.UpdateAsync(id, dto);
        return guard == null
            ? NotFound(new { Message = $"No se encontró el vigilador con Id {id}." })
            : Ok(guard);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"No se encontró el vigilador con Id {id}." });
    }

    [HttpGet("by-workplace/{workplaceId}")]
    public async Task<ActionResult<IEnumerable<SecurityGuardDTO>>> GetByWorkplace(int workplaceId)
        => Ok(await _service.GetByWorkplaceAsync(workplaceId));
}
