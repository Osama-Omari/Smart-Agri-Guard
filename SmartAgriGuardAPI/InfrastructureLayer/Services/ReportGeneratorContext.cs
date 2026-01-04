using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class ReportGeneratorContext
    {
        private IReportStrategy _strategy;

        public ReportGeneratorContext(IReportStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<(byte[] FileContent, string FileName, string ContentType)> GenerateAsync(ReportDataDTO reportData , string userTimeZoneId)
        {
            var fileContent = await _strategy.GenerateReportAsync(reportData, userTimeZoneId);
            var fileName = $"Report_{reportData.GreenhouseName}_{DateTime.UtcNow:yyyyMMdd}{_strategy.FileExtension}";
            return (fileContent, fileName, _strategy.ContentType);
        }

        public void SetStrategy(IReportStrategy strategy)
        {
            this._strategy = strategy;
        }

        public IReportStrategy GetStrategy()
        {
            return _strategy;
        }

       
    }
}
