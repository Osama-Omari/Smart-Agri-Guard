using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DocumentFormat.OpenXml.Validation;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportServcie _reportService;
        public ReportController(IReportServcie reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("Generate")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequestDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var reportData = await _reportService.BuildReportDataAsync(dto);

                IReportStrategy strategy = dto.ReportFormat.ToLower() switch
                {
                    "pdf" => new InfrastructureLayer.Services.PdfReportStrategy(),
                    "excel" => new InfrastructureLayer.Services.ExcelReportStrategy(),
                    _ => throw new ArgumentException("Unsupported report format.")
                };

                var generator = new ReportGenerator(strategy);
                var result = await generator.GenerateAsync(reportData);

                return File(result.FileContent, result.ContentType, result.FileName);

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
