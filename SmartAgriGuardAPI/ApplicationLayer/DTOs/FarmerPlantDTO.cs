using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class FarmerPlantDTO
    {

        [Required]
        public List<Guid> assignedPlants {  get; set; }
    }
}
