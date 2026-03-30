using Microsoft.AspNetCore.Mvc;

namespace SGV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Verifica que la API esté funcionando correctamente.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "OK",
            Service = "SGV API",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow
        });
    }
}
