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
    public class PlantNotificationsRepository : IPlantNotificationsRepository
    {
        private readonly SmartAgriGuardDbContext _context;
        public PlantNotificationsRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

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

        public async Task<List<PlantNotifications>> GetByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.PlantNotifications
                    .Where(n => n.PlantId == plantId)
                    .Include(n => n.Plant)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving plant notifications.", ex);
            }
        }

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
    }
}
