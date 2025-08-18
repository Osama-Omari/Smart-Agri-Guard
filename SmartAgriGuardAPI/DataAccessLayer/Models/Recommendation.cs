using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Recommendation
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }

        public Plant Plant { get; set; }

        public string advice { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool isCritical { get; set; }


    }
}
