using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SystemReports
    {
        public Guid Id { get; set; }
        public Guid GreenhouseId { get; set; }

        public Greenhouse Greenhouse { get; set; }

        public string ErrorType { get; set; }

        public string Message { get; set; } 

        public bool IsRead { get; set; }

        public DateTimeOffset ReportDate { get; set; }

    }
}
