using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository responsible for persisting and managing plant-specific alerts and notifications.
    /// Tracks the status (Read/Unread) of environmental warnings for farmers.
    /// </summary>
    public class PlantNotificationsRepository : IPlantNotificationsRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public PlantNotificationsRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves a new notification record to the database.
        /// </summary>
        /// <param name="plantNotification">The notification entity to be added.</param>
        public async Task AddAsync(PlantNotifications plantNotification)
        {
            try
            {
                await _context.PlantNotifications.AddAsync(plantNotification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the plant notification.", ex);
            }
        }

        /// <summary>
        /// Permanently removes a notification from the system.
        /// </summary>
        /// <param name="notificationId">The GUID of the notification.</param>
        public async Task DeleteAsync(Guid notificationId)
        {
            try
            {
                var notification = await _context.PlantNotifications.FirstOrDefaultAsync(n => n.Id == notificationId);
                if (notification != null)
                {
                    _context.PlantNotifications.Remove(notification);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Plant notification not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the plant notification.", ex);
            }
        }

        /// <summary>
        /// Retrieves a batch of notifications based on a list of IDs.
        /// Commonly used for bulk operations like "Mark all as read".
        /// </summary>
        public async Task<List<PlantNotifications>> GetByIdsAsync(List<Guid> notificationIds)
        {
            try
            {
                return await _context.PlantNotifications
                    .Where(n => notificationIds.Contains(n.Id))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving plant notifications by IDs.", ex);
            }
        }

        /// <summary>
        /// Retrieves all notifications for a specific plant, ordered by the most recent first.
        /// Includes the related Plant data via Eager Loading.
        /// </summary>
        public async Task<List<PlantNotifications>> GetByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.PlantNotifications
                    .Where(n => n.PlantId == plantId)
                    .OrderByDescending(x => x.NotificationDate)
                    .Include(n => n.Plant)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving plant notifications.", ex);
            }
        }

        /// <summary>
        /// Updates the status of a specific notification to 'Read'.
        /// </summary>
        public async Task MarkAsReadAsync(Guid notificationId)
        {
            try
            {
                var notification = await _context.PlantNotifications.FirstOrDefaultAsync(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    _context.PlantNotifications.Update(notification);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Plant notification not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking the notification as read.", ex);
            }
        }

        /// <summary>
        /// Updates an existing notification record with new information.
        /// </summary>
        public async Task UpdateAsync(PlantNotifications plantNotifications)
        {
            try
            {
                _context.PlantNotifications.Update(plantNotifications);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the plant notification.", ex);
            }
        }
    }
}