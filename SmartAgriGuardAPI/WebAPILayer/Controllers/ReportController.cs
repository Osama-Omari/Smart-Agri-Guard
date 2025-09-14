using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpGet("Generate")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequestDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // Placeholder for report generation logic
                await Task.Delay(1000); // Simulate async work
                return Ok(new { Message = "Report generated successfully." });


            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
