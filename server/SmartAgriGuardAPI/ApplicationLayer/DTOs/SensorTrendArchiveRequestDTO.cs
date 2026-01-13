using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class SensorTrendArchiveRequestDTO
    {
        public Guid PlantId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public List<string> Metrics { get; set; } = new List<string>();

    }
}
