using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class SensorDataRegisterDTO
    {
        public DateTimeOffset Timestamp { get; set; }
        public double? Temperature { get; set; }

        public double? Humidity { get; set; }

        public double? SoilMoisture { get; set; }

        public double? PH { get; set; }

        public double? Potassium { get; set; }

        public double? Phosphorus { get; set; }

        public double? Nitrogen { get; set; }
    }
}
