using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class AssignedFarmerDTO
    {
        public Guid FarmerId { get; set; }

        public string FullName { get; set; }    

        public string UserName { get; set; }

    }
}
