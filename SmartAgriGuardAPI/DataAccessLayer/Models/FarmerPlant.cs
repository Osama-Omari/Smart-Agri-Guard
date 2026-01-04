using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class FarmerPlant
    {
        public Guid FarmerId { get; set; }
        public User Farmer { get; set; }
        public Guid PlantId { get; set; }
        public Plant Plant { get; set; }

        public DateTimeOffset AssignedAt { get; set; }
    }
}
