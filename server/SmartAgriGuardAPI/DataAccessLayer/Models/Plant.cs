using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Plant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Location { get; set; }

        public string? ImageUrl { get; set; }

        public Guid GreenhouseId { get; set; }

        public Greenhouse Greenhouse { get; set; }

        public Guid PlantTypeId { get; set; }

        public PlantType PlantType { get; set; }

        public List<SensorData> SensorData { get; set; }

        public List<SensorDataArchive> SensorDataArchives { get; set; }

        public List<Prediction> Predictions { get; set; }

        public List<PlantNotifications> PlantNotifications { get; set; }


        public List<FarmerPlant> FarmerPlants { get; set; }

        public List<PlantSchedule> PlantSchedules { get; set; } = new();
    }
}
