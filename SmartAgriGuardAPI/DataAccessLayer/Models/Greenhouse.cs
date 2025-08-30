using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Greenhouse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string ImageUrl { get; set; }

        public Guid? ManagerId { get; set; }

        public User Manager { get; set; }

        public List<Plant> Plants { get; set; }

        public List<User> Farmers { get; set; }
    }
}
