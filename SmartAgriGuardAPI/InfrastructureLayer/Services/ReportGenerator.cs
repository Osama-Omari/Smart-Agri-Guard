using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class ReportGenerator
    {
        private readonly IReportStrategy _strategy;

        public ReportGenerator(IReportStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<(byte[] FileContent, string FileName, string ContentType)> GenerateAsync(ReportDataDTO reportData)
        {
            var fileContent = await _strategy.GenerateReportAsync(reportData);
            var fileName = $"Report_{DateTime.UtcNow:yyyyMMddHHmmss}{_strategy.FileExtension}";
            return (fileContent, fileName, _strategy.ContentType);
        }
    }
}
