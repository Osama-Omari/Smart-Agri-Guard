using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DeviceToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }

        public string DeviceType { get; set; }

        public string? DeviceModel { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTimeOffset? LastUpdated { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
