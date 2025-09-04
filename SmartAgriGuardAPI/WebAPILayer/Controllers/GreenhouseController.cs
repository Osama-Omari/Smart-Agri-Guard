using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.RequestDTO;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GreenhouseController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IGreenhouseService _greenhouseService;
       
        public GreenhouseController(IFileStorageService fileStorageService, IGreenhouseService greenhouseService)
        {
            _fileStorageService = fileStorageService;
            _greenhouseService = greenhouseService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> CreateGreenhouse([FromForm] CreateGreenhouseRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string? imagePath = null;
                if (dto.Image is { Length: > 0 })
                {
                    await using var stream = dto.Image.OpenReadStream();
                    var filedata = new FileDataDTO
                    {
                        Content = stream,
                        FileName = dto.Image.FileName
                    };
                    imagePath = await _fileStorageService.SaveFileAsync(filedata, "greenhouses");
                }
                var greenhouseRegisterDTO = new GreenhouseRegisterDTO
                {
                    Location = dto.Location,
                    Name = dto.Name,
                    ImagePath = imagePath
                };
                var result = await _greenhouseService.AddGreenhouse(greenhouseRegisterDTO);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }





        }

    }
}
