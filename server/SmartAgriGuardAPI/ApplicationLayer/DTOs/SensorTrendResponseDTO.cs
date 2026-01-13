using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class SensorTrendResponseDTO
    {
        public Guid PlantId { get; set; }
        public List<Dictionary<string, object>> Readings { get; set; }
    }
}
