using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantWithMetricsDTO
    {
        public Guid Id { get; set; }

        public string PlantName { get; set; }

        public string? Location { get; set; }

        public string? Image { get; set; }

        public SensorMetricDTO LatestMetrics { get; set; }
    }
}
