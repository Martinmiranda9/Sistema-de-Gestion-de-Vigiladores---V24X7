using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.Objetivo;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObjetivosController : ControllerBase
{
    private readonly IObjetivoService _objetivoService;

    public ObjetivosController(IObjetivoService objetivoService)
    {
        _objetivoService = objetivoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ObjetivoDTO>>> GetAll()
    {
        var objetivos = await _objetivoService.GetAllAsync();
        return Ok(objetivos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ObjetivoDTO>> GetById(int id)
    {
        var objetivo = await _objetivoService.GetByIdAsync(id);
        if (objetivo == null)
            return NotFound(new { Message = $"No se encontró el objetivo con Id {id}." });

        return Ok(objetivo);
    }

    [HttpPost]
    public async Task<ActionResult<ObjetivoDTO>> Create([FromBody] ObjetivoCreateDTO dto)
    {
        var objetivo = await _objetivoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = objetivo.Id }, objetivo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ObjetivoDTO>> Update(int id, [FromBody] ObjetivoUpdateDTO dto)
    {
        var objetivo = await _objetivoService.UpdateAsync(id, dto);
        if (objetivo == null)
            return NotFound(new { Message = $"No se encontró el objetivo con Id {id}." });

        return Ok(objetivo);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _objetivoService.DeleteAsync(id);
        if (!result)
            return NotFound(new { Message = $"No se encontró el objetivo con Id {id}." });

        return NoContent();
    }
}
