using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SensorData
    {
        public Guid Id { get; set; }

        public Guid PlantId { get; set; }

        public Plant Plant { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double SoilMoisture { get; set; }

        public double Nitrogen { get; set; }

        public double Phosphorus { get; set; }

        public double Potassium { get; set; }

        public double Ph { get; set; }

        public DateTime Timestamp { get; set; }


    }
}
