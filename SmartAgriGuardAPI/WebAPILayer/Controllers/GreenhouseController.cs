using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GreenhouseController : ControllerBase
    {
        //[HttpPost("Add")]
        //public async Task<IActionResult> CreateGreenhouse([FromForm] GreenhouseRegisterDTO dto)
        //{ 




        //}

    }
}
