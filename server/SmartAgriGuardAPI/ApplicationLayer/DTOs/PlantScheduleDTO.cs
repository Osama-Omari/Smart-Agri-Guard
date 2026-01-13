using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantScheduleDTO
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public string TaskType { get; set; }
        public string Frequency { get; set; }
        public List<DayOfWeek>? Days { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool IsActive { get; set; }
    }
}
