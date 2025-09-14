using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class ReportRequestDTO
    {
        public Guid GreenhouseId { get; set; }

        public List<Guid> PlantIds { get; set; } 

        public List<string> SensorTypes { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ReportFormat { get; set; } 
    }
}
