using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class CreateScheduleDTO
    {
        public string TaskType { get; set; }    

        public string Frequency { get; set; }

        public List<string>? Days { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }


    }
}
