using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class ManagerRegisterDTO
    {
        public string FullName { get; set; }    

        public string UserName { get; set; }

        public string Password { get; set; }

        public List<Guid>? GreenhousesIds { get; set; }

    }
}
