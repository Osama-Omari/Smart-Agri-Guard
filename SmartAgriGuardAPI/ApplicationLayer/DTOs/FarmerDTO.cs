using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class FarmerDTO
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public Guid GreenhouseId { get; set; }

        //public List<string>? AssignedPlantsNames { get; set; }


    }
}
