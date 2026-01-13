using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class ReportDataDTO
    {
        public string GreenhouseName { get; set; }
        public List<string> SelectedSensorTypes { get; set; } = new();
        public List<PlantReportDTO> Plants { get; set; } = new();
    }
}
