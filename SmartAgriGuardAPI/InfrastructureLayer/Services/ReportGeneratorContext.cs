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
    /// <summary>
    /// Acts as the Context in the Strategy Design Pattern for report generation.
    /// This class maintains a reference to a specific <see cref="IReportStrategy"/> 
    /// and delegates the actual generation logic to it.
    /// </summary>
    public class ReportGeneratorContext
    {
        private IReportStrategy _strategy;

        /// <summary>
        /// Initializes the context with a specific reporting strategy (e.g., PDF or Excel).
        /// </summary>
        /// <param name="strategy">The concrete strategy implementation to use.</param>
        public ReportGeneratorContext(IReportStrategy strategy)
        {
            _strategy = strategy;
        }

        /// <summary>
        /// Executes the generation process using the current strategy and prepares the file metadata.
        /// </summary>
        /// <param name="reportData">The aggregated data to be included in the report.</param>
        /// <param name="userTimeZoneId">The timezone ID used to localize timestamps within the file.</param>
        /// <returns>
        /// A tuple containing the raw byte array of the file, a dynamically generated file name, 
        /// and the appropriate HTTP Content-Type.
        /// </returns>
        public async Task<(byte[] FileContent, string FileName, string ContentType)> GenerateAsync(ReportDataDTO reportData, string userTimeZoneId)
        {
            // Delegate the generation of the byte array to the selected strategy
            var fileContent = await _strategy.GenerateReportAsync(reportData, userTimeZoneId);

            // Construct a meaningful file name using the Greenhouse name and current date
            // Example: Report_MainGreenhouse_20240520.pdf
            var fileName = $"Report_{reportData.GreenhouseName}_{DateTime.UtcNow:yyyyMMdd}{_strategy.FileExtension}";

            return (fileContent, fileName, _strategy.ContentType);
        }

        /// <summary>
        /// Allows the reporting strategy to be changed at runtime if necessary.
        /// </summary>
        /// <param name="strategy">The new strategy implementation.</param>
        public void SetStrategy(IReportStrategy strategy)
        {
            this._strategy = strategy;
        }

        /// <summary>
        /// Retrieves the strategy currently being used by the context.
        /// </summary>
        /// <returns>The current <see cref="IReportStrategy"/>.</returns>
        public IReportStrategy GetStrategy()
        {
            return _strategy;
        }
    }
}