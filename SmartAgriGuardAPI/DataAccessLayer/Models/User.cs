using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string username { get; set; }

        public byte[] PasswordHash { get; set; }

        public Guid UserRoleId { get; set; }

        public UserRole UserRole { get; set; }

        public Guid? GreenhouseId { get; set; }

        public Greenhouse Greenhouse { get; set; }

        public List<Greenhouse>? ManagedGreenhouses { get; set; } = new List<Greenhouse>();

        public List<FarmerPlant> FarmerPlants { get; set; } = new List<FarmerPlant>();

        public List<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();



    }
}
