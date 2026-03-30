using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using SGV.DTOs.ConfiguracionLiquidacion;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfiguracionesLiquidacionController : ControllerBase
{
    private readonly IConfiguracionLiquidacionService _service;

    public ConfiguracionesLiquidacionController(IConfiguracionLiquidacionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConfiguracionLiquidacionDTO>>> GetAll()
    {
        var configs = await _service.GetAllAsync();
        return Ok(configs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConfiguracionLiquidacionDTO>> GetById(int id)
    {
        var config = await _service.GetByIdAsync(id);
        if (config == null)
            return NotFound(new { Message = $"No se encontró la configuración con Id {id}." });

        return Ok(config);
    }

    [HttpPost]
    public async Task<ActionResult<ConfiguracionLiquidacionDTO>> Create([FromBody] ConfiguracionLiquidacionCreateDTO dto)
    {
        var config = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = config.Id }, config);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ConfiguracionLiquidacionDTO>> Update(int id, [FromBody] ConfiguracionLiquidacionUpdateDTO dto)
    {
        var config = await _service.UpdateAsync(id, dto);
        if (config == null)
            return NotFound(new { Message = $"No se encontró la configuración con Id {id}." });

        return Ok(config);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound(new { Message = $"No se encontró la configuración con Id {id}." });

        return NoContent();
    }

    /// <summary>
    /// Obtiene la configuración de precios vigente a una fecha dada.
    /// </summary>
    [HttpGet("vigente")]
    public async Task<ActionResult<ConfiguracionLiquidacionDTO>> GetVigente([FromQuery] DateTime fecha)
    {
        var config = await _service.GetVigenteAsync(fecha);
        if (config == null)
            return NotFound(new { Message = "No hay configuración vigente para esa fecha." });

        return Ok(config);
    }
}
