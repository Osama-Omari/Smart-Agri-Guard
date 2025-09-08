using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantDTO
    {
        public Guid Id { get; set; }
        public string PlantName { get; set; }

        public string PlantTypeName { get; set; }

        public string GreenhouseName { get; set; }
        public string? Location { get; set; }
    }
}
