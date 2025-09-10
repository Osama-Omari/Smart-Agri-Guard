using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class GreenhouseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Location { get; set; }

        public List<FarmerDTO> Farmers { get; set; }

        public List<PlantDTO> Plants { get; set; }

    }
}
