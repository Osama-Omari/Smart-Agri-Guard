using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class PlantSchedule
    {
        public Guid Id { get; set; }

        public Guid PlantId { get; set; }

        public Plant Plant { get; set; }

        public string TaskType { get; set; }

        public string Frequency { get; set; }

        public string? DaysOfWeek { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public string CronExpression { get; set; } 

        public bool IsActive { get; set; }
    }
}
