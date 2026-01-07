using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Background job service responsible for database maintenance and data retention policies.
    /// Handles notification cleanup, sensor data archiving, and device token management.
    /// </summary>
    public class DataCleanupJob : IDataCleanupJob
    {
        private readonly IPlantNotificationsRepository _notificationsRepository;
        private readonly ISystemReportsRepository _systemReportsRepository;
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly ISensorDataArchiveRepository _sensorDataArchiveRepository;
        private readonly ISensorDataRepository _sensorDataRepository;

        public DataCleanupJob(IPlantNotificationsRepository notificationsRepository,
                              ISystemReportsRepository systemReportsRepository,
                              IDeviceTokenRepository deviceTokenRepository,
                              ISensorDataArchiveRepository sensorDataArchiveRepository,
                              ISensorDataRepository sensorDataRepository)
        {
            _notificationsRepository = notificationsRepository;
            _systemReportsRepository = systemReportsRepository;
            _deviceTokenRepository = deviceTokenRepository;
            _sensorDataArchiveRepository = sensorDataArchiveRepository;
            _sensorDataRepository = sensorDataRepository;
        }

        /// <summary>
        /// Cleans up the database by deleting plant notifications and system reports 
        /// that have already been read by users.
        /// </summary>
        public async Task ClearReadNotificationsAndReports()
        {
            // Retrieve and batch-delete read plant-specific alerts
            var readNotifications = await _notificationsRepository.GetReadPlantsNotifications();
            if (readNotifications != null && readNotifications.Any())
            {
                await _notificationsRepository.DeleteRangeAsync(readNotifications);
            }

            // Retrieve and batch-delete read greenhouse system reports
            var readReports = await _systemReportsRepository.GetReadSystemReports();
            if (readReports != null && readReports.Any())
            {
                await _systemReportsRepository.DeleteSystemReports(readReports);
            }
        }

        /// <summary>
        /// Orchestrates the multi-stage lifecycle of sensor telemetry.
        /// 1. Moves data older than 2 months from live storage to Archive storage.
        /// 2. Permanently deletes archived data older than 1 year.
        /// </summary>
        public async Task ProcessSensorDataLifecycle()
        {
            var twoMonthsAgo = DateTimeOffset.UtcNow.AddMonths(-2);
            var oneYearAgo = DateTimeOffset.UtcNow.AddYears(-1);

            // Fetch 'Warm' data that needs to be moved to 'Cold' storage
            var dataToArchive = await _sensorDataRepository.GetSensorDataOlderThan(twoMonthsAgo);

            if (dataToArchive != null && dataToArchive.Any())
            {
                // Map live sensor data to the Archive model
                var archiveEntries = dataToArchive.Select(data => new DataAccessLayer.Models.SensorDataArchive
                {
                    Id = data.Id,
                    PlantId = data.PlantId,
                    Timestamp = data.Timestamp,
                    Temperature = data.Temperature,
                    Humidity = data.Humidity,
                    SoilMoisture = data.SoilMoisture,
                    Nitrogen = data.Nitrogen,
                    Phosphorus = data.Phosphorus,
                    Potassium = data.Potassium,
                    Ph = data.Ph,
                    ArchivedAt = DateTimeOffset.UtcNow
                }).ToList();

                // Move data: Add to Archive, then remove from Live
                await _sensorDataArchiveRepository.AddRange(archiveEntries);
                await _sensorDataRepository.RemoveRange(dataToArchive);

                // Permanent Deletion: Remove archives that have exceeded the 1-year retention limit
                var expiredArchives = await _sensorDataArchiveRepository.GetSensorDataArchivesOlderThan(oneYearAgo);
                if (expiredArchives != null && expiredArchives.Any())
                {
                    await _sensorDataArchiveRepository.RemoveRange(expiredArchives);
                }
            }
        }

        /// <summary>
        /// Maintenance task to remove device tokens that are no longer valid or marked as inactive.
        /// Helps maintain the efficiency of the Firebase Cloud Messaging broadcast lists.
        /// </summary>
        public async Task RemoveInactiveDeviceTokens()
        {
            var inactiveDeviceTokens = await _deviceTokenRepository.GetInactiveDeviceTokens();
            if (inactiveDeviceTokens != null && inactiveDeviceTokens.Any())
            {
                await _deviceTokenRepository.DeleteRangeAsync(inactiveDeviceTokens);
            }
        }
    }
}