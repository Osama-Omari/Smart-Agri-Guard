using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Prediction
    {
        public Guid Id { get; set; }

        public Guid PlantId { get; set; }

        public Plant Plant { get; set; }

        public DateTimeOffset PredictionDate { get; set; }

        public string healthStatus { get; set; }



    }
}
