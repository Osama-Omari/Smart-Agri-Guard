using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class LogoutRequestDTO
    {
        [Required(ErrorMessage = "DeviceToken is required")]
        public string DeviceToken { get; set; }
    }
}
