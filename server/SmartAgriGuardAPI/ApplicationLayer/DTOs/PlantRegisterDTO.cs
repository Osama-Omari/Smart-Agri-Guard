using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantRegisterDTO
    {
        public string Name { get; set; }

        public Guid PlantTypeId { get; set; }

        public string? Location { get; set; }

        public string? ImagePath { get; set; }
    }
}
