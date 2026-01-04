using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantWithAssignedFarmersDTO
    {
        public Guid PlantId { get; set; }

        public string PlantName { get; set; }

        public string? Location { get; set; }

        public List<AssignedFarmerDTO> Farmers { get; set; }

        
    }
}
