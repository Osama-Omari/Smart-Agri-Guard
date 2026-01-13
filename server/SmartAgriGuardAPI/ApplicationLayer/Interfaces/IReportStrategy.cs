using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IReportStrategy
    {
        Task<byte[]> GenerateReportAsync(ReportDataDTO reportData , string userTimeZoneId);
        string FileExtension { get; }
        string ContentType { get; }
    }
}

