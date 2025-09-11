using System.ComponentModel.DataAnnotations;

namespace WebAPILayer.RequestDTO
{
    public class UpdateGreenhouseRequestDTO
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters.")]
        public string? Name { get; set; }

        [StringLength(300, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 300 characters.")]
        public string? Location { get; set; }


        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }
    }
}
