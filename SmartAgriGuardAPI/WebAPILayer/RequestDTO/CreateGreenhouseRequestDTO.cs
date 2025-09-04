using System.ComponentModel.DataAnnotations;

namespace WebAPILayer.RequestDTO
{
    public class CreateGreenhouseRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200,ErrorMessage = "Name for the Greenhouse can't exceed 200 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Location is required")]
        [StringLength(300,ErrorMessage = "Location can't exceed 300 characters")]
        public string Location { get; set; }

        public IFormFile? Image { get; set; }
    }
}
