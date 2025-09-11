using System.ComponentModel.DataAnnotations;

namespace WebAPILayer.RequestDTO
{
    public class UpdatePlantRequestDTO
    {
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Location must be less than 200 characters.")]
        public string? Location { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }
    }
}
