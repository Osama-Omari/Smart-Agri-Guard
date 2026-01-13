using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantReportDTO
    {
        public Guid PlantId { get; set; }
        public string PlantName { get; set; }
        public List<SensorRowDTO> SensorData { get; set; } = new();
    }
}
