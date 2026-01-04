using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class PlantNotifications
    {
        public Guid Id { get; set; }

        public Guid PlantId { get; set; }

        public Plant Plant { get; set; }

        public DateTimeOffset NotificationDate { get; set; }

        public string TriggerType { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }
    }
}
