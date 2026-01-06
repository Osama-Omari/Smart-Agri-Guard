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
    public class PlantScheduleRepository : IPlantScheduleRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public PlantScheduleRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        public async Task AddPlantScheduleAsync(PlantSchedule plantSchedule)
        {
            try
            {
                await _context.PlantSchedules.AddAsync(plantSchedule);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { 
                throw new Exception("Error adding plant schedule", ex);
            }
        }

        public async Task DeletePlantScheduleAsync(Guid id)
        {
            try
            {
                var plantSchedule = await _context.PlantSchedules.FindAsync(id);
                if (plantSchedule != null)
                {
                    _context.PlantSchedules.Remove(plantSchedule);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex) {
                throw new Exception("Error deleting plant schedule", ex);
            }
        }

        public async Task<IEnumerable<PlantSchedule>> GetAllPlantSchedulesAsync()
        {
            try
            {
                return await _context.PlantSchedules.ToListAsync();

            }
            catch (Exception ex) {
                throw new Exception("Error retrieving plant schedules", ex);
            }
        }

        public async Task<PlantSchedule?> GetPlantScheduleByIdAsync(Guid id)
        {
            try
            {
                return await _context.PlantSchedules.FindAsync(id);
            }
            catch (Exception ex) {
                throw new Exception("Error retrieving plant schedule by ID", ex);
            }
        }

        public async Task<IEnumerable<PlantSchedule>> GetPlantSchedulesByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.PlantSchedules
                    .Where(ps => ps.PlantId == plantId)
                    .ToListAsync();
            }
            catch (Exception ex) {
                throw new Exception("Error retrieving plant schedules by plant ID", ex);
            }
        }

        public async Task UpdatePlantScheduleAsync(PlantSchedule plantSchedule)
        {
            try
            {
                _context.PlantSchedules.Update(plantSchedule);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new Exception("Error updating plant schedule", ex);
            }
        }
    }
}
