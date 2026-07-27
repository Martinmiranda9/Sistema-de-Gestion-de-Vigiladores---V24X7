using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGV.Business.Interfaces;
using System.Threading.Tasks;

namespace SGV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("ProcessAttendanceSheet")]
        public async Task<IActionResult> ProcessAttendanceSheet(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image was provided.");

            try
            {
                using var stream = image.OpenReadStream();
                var result = await _geminiService.ProcessAttendanceSheetAsync(stream, image.ContentType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // In production, log the exception.
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
