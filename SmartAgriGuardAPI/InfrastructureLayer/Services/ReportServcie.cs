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

            var archiveThreshold = DateTime.UtcNow.AddMonths(-2);

            foreach (var plantId in request.PlantIds)
            {
                var plant = await _plantRepository.GetPlantById(plantId);
                if (plant == null) continue;

                List<SensorData> mergedData = new();

                // Case 1: Entire range in archive
                if (request.EndDate <= archiveThreshold)
                {
                    var archiveData = await _sensorDataArchiveRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, request.StartDate, request.EndDate);

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
                // Case 2: Entire range in live SensorData
                else if (request.StartDate >= archiveThreshold)
                {
                    mergedData = (await _sensorDataRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, request.StartDate, request.EndDate)).ToList();
                }
                // Case 3: Range spans both archive and live
                else
                {
                    var archiveData = await _sensorDataArchiveRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, request.StartDate, archiveThreshold);
                    var liveData = await _sensorDataRepository.GetByPlantIdAndDateRangeAsync(
                        plantId, archiveThreshold, request.EndDate);

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

                    mergedData = archiveDataAsSensorData.Concat(liveData).OrderBy(d => d.Timestamp).ToList();
                }

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
