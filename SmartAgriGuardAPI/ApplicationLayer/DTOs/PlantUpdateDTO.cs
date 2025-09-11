using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantUpdateDTO
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? ImagePath { get; set; }
    }
}
