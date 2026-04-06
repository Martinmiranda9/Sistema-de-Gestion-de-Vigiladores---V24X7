using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.Workplace;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkplacesController : ControllerBase
{
    private readonly IWorkplaceService _service;

    public WorkplacesController(IWorkplaceService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkplaceDTO>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkplaceDTO>> GetById(int id)
    {
        var workplace = await _service.GetByIdAsync(id);
        return workplace == null
            ? NotFound(new { Message = $"No se encontró el objetivo con Id {id}." })
            : Ok(workplace);
    }

    [HttpPost]
    public async Task<ActionResult<WorkplaceDTO>> Create([FromBody] WorkplaceCreateDTO dto)
    {
        var workplace = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = workplace.Id }, workplace);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WorkplaceDTO>> Update(int id, [FromBody] WorkplaceUpdateDTO dto)
    {
        var workplace = await _service.UpdateAsync(id, dto);
        return workplace == null
            ? NotFound(new { Message = $"No se encontró el objetivo con Id {id}." })
            : Ok(workplace);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"No se encontró el objetivo con Id {id}." });
    }
}
