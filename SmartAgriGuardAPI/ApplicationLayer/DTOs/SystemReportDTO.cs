using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class SystemReportDTO
    {
        public Guid Id { get; set; }
        public string GreenhouseName { get; set; }  
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public DateTimeOffset ReportDate { get; set; }
        public bool IsRead { get; set; }
    }
}
