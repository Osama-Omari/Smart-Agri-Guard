using System.ComponentModel.DataAnnotations;

namespace WebAPILayer.RequestDTO
{
    public class CreatePlantRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, ErrorMessage = "Name for the Greenhouse can't exceed 200 characters")]
        public string PlantName { get; set; }

        [Required]
        public Guid PlantTypeId { get; set; }
        [StringLength(300,ErrorMessage = "Location for the plant can't exceed 300 characters")]
        public string? Location { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }



    }
}
