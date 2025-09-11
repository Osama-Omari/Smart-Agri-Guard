using System.ComponentModel.DataAnnotations;

namespace WebAPILayer.RequestDTO
{
    public class CreateGreenhouseRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters.")]

        public string Name { get; set; }
        [Required(ErrorMessage = "Location is required")]
        [StringLength(300,ErrorMessage = "Location can't exceed 300 characters")]
        public string Location { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }
    }
}
