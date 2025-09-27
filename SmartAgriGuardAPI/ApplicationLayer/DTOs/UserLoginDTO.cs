using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class UserLoginDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string DeviceToken { get; set; }

        public string DeviceType { get; set; }

        public string? DeviceModel { get; set; }
    }
}
