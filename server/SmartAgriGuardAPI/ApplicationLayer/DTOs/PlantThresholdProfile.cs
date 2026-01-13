using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantThresholdProfile
    {
        // Temperature (°C)
        public double TempMin { get; set; }
        public double TempMax { get; set; }

        // Relative humidity (%)
        public double HumidityMin { get; set; }

        // Soil moisture (% or sensor index – adjust to your sensor scale)
        public double SoilMoistureLow { get; set; }

        // NPK “low” threshold – depends on your sensor units (example values)
        public double NLow { get; set; }
        public double PLow { get; set; }
        public double KLow { get; set; }

        // Soil pH
        public double PhMin { get; set; }
        public double PhMax { get; set; }
    }
}
