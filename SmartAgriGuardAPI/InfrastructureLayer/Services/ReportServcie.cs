using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service responsible for aggregating and preparing data for report generation.
    /// It intelligently merges real-time sensor data with historical archives based on requested date ranges.
    /// </summary>
    public class ReportServcie : IReportServcie
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly ISensorDataArchiveRepository _sensorDataArchiveRepository;
        private readonly IGreenhouseRepository _greenhouseRepository;
        private readonly IPlantRepository _plantRepository;

        public ReportServcie(
            ISensorDataRepository sensorDataRepository,
            ISensorDataArchiveRepository sensorDataArchiveRepository,
            IGreenhouseRepository greenhouseRepository,
            IPlantRepository plantRepository)
        {
            _sensorDataRepository = sensorDataRepository;
            _sensorDataArchiveRepository = sensorDataArchiveRepository;
            _greenhouseRepository = greenhouseRepository;
            _plantRepository = plantRepository;
        }

        /// <summary>
        /// Builds the comprehensive data structure required for a greenhouse report.
        /// </summary>
        /// <remarks>
        /// The logic uses an 'archiveThreshold' (set to 2 months ago). Data is retrieved from 
        /// either the Live table, the Archive table, or concatenated from both, ensuring 
        /// report continuity across data migration boundaries.
        /// </remarks>
        /// <param name="request">The report parameters including date range and specific plants.</param>
        /// <returns>A populated ReportDataDTO ready for the reporting strategies (PDF/Excel).</returns>
        /// <exception cref="ArgumentException">Thrown if the greenhouse ID is invalid.</exception>
        public async Task<ReportDataDTO> BuildReportDataAsync(ReportRequestDTO request)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(request.GreenhouseId);
            if (greenhouse == null)
                throw new ArgumentException("Invalid greenhouse ID.");

            var report = new ReportDataDTO
            {
                GreenhouseName = greenhouse.Name,
                SelectedSensorTypes = request.SensorTypes
            };

            // Define the boundary between "Live" data and "Archived" data
            var archiveThreshold = DateTime.UtcNow.AddMonths(-2);

            foreach (var plantId in request.PlantIds)
            {
                var plant = await _plantRepository.GetPlantById(plantId);
                if (plant == null) continue;

                List<SensorData> mergedData = new();

                // Scenario 1: The entire requested range exists in the Archive storage
                if (request.EndDate <= archiveThreshold)
                {
                    var archiveData = await _sensorDataArchiveRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, request.StartDate, request.EndDate);

                    // Manual mapping from Archive Model to SensorData Model for consistency
                    mergedData = archiveData.Select(a => new SensorData
                    {
                        Id = a.Id,
                        PlantId = a.PlantId,
                        Plant = a.Plant,
                        Temperature = a.Temperature,
                        Humidity = a.Humidity,
                        SoilMoisture = a.SoilMoisture,
                        Nitrogen = a.Nitrogen,
                        Phosphorus = a.Phosphorus,
                        Potassium = a.Potassium,
                        Ph = a.Ph,
                        Timestamp = a.Timestamp
                    }).ToList();
                }
                // Scenario 2: The entire requested range exists in the Live storage
                else if (request.StartDate >= archiveThreshold)
                {
                    mergedData = (await _sensorDataRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, request.StartDate, request.EndDate)).ToList();
                }
                // Scenario 3: The requested range spans across the threshold (starts in Archive, ends in Live)
                else
                {
                    var archiveData = await _sensorDataArchiveRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, request.StartDate, archiveThreshold);
                    var liveData = await _sensorDataRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, archiveThreshold, request.EndDate);

                    // Map and concatenate both data sources
                    var archiveDataAsSensorData = archiveData.Select(a => new SensorData
                    {
                        Id = a.Id,
                        PlantId = a.PlantId,
                        Plant = a.Plant,
                        Temperature = a.Temperature,
                        Humidity = a.Humidity,
                        SoilMoisture = a.SoilMoisture,
                        Nitrogen = a.Nitrogen,
                        Phosphorus = a.Phosphorus,
                        Potassium = a.Potassium,
                        Ph = a.Ph,
                        Timestamp = a.Timestamp
                    });

                    // Merge and sort by timestamp to ensure chronological order in the report
                    mergedData = archiveDataAsSensorData.Concat(liveData).OrderBy(d => d.Timestamp).ToList();
                }

                // Add the processed plant data to the final report collection
                report.Plants.Add(new PlantReportDTO
                {
                    PlantId = plantId,
                    PlantName = plant.Name,
                    SensorData = mergedData.Select(d => new SensorRowDTO
                    {
                        Timestamp = d.Timestamp,
                        Temperature = d.Temperature,
                        Humidity = d.Humidity,
                        SoilMoisture = d.SoilMoisture,
                        Nitrogen = d.Nitrogen,
                        Phosphorus = d.Phosphorus,
                        Potassium = d.Potassium,
                        Ph = d.Ph
                    }).ToList()
                });
            }

            return report;
        }
    }
}