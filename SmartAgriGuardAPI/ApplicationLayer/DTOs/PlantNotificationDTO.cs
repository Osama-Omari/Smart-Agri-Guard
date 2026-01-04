using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class PlantNotificationDTO
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }

        public string PlantName { get; set; }   
        public DateTimeOffset NotificationDate { get; set; }
        public string TriggerType { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }

    }
}
