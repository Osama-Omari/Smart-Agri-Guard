using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DocumentFormat.OpenXml.Validation;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Handles the generation and exportation of greenhouse and plant reports.
    /// Utilizes the Strategy Pattern to support multiple export formats like PDF and Excel.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportServcie _reportService;

        public ReportController(IReportServcie reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Generates a report based on the provided criteria and format.
        /// </summary>
        /// <param name="dto">The report request details, including date ranges and the desired format ("pdf" or "excel").</param>
        /// <returns>A file stream of the generated report.</returns>
        /// <response code="200">Returns the generated file (PDF or Excel).</response>
        /// <response code="400">If the format is unsupported or the request data is invalid.</response>
        /// <response code="401">Unauthorized if the user is not a Manager.</response>
        [HttpPost("Generate")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // Select the appropriate reporting strategy based on the requested format
                IReportStrategy strategy = dto.ReportFormat.ToLower() switch
                {
                    "pdf" => new InfrastructureLayer.Services.PdfReportStrategy(),
                    "excel" => new InfrastructureLayer.Services.ExcelReportStrategy(),
                    _ => throw new ArgumentException("Unsupported report format. Please use 'pdf' or 'excel'.")
                };

                // Retrieve timezone from claims to ensure report timestamps match the manager's local time
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value ?? "UTC";

                // Fetch data for the report
                var reportData = await _reportService.BuildReportDataAsync(dto);

                // Execute the generation context with the chosen strategy
                var generator = new ReportGeneratorContext(strategy);
                var result = await generator.GenerateAsync(reportData, userTimeZoneId);

                // Returns the file directly to the browser/client for download
                return File(result.FileContent, result.ContentType, result.FileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred during report generation: {ex.Message}");
            }
        }
    }
}