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
    /// Repository managing the data access for Greenhouse entities.
    /// Provides methods for handling facility infrastructure, staff assignments, and plant inventories.
    /// </summary>
    public class GreenhouseRepository : IGreenhouseRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public GreenhouseRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a new greenhouse facility to the database.
        /// </summary>
        /// <param name="greenhouse">The greenhouse entity to create.</param>
        public async Task AddAsync(Greenhouse greenhouse)
        {
            try
            {
                await _context.Greenhouses.AddAsync(greenhouse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding GreenHouse: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a greenhouse record from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the greenhouse to remove.</param>
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var greenhouse = await _context.Greenhouses.FindAsync(id);
                if (greenhouse != null)
                {
                    _context.Greenhouses.Remove(greenhouse);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while deleting greenhouse: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all greenhouses in the system, including their associated plants and farmers.
        /// </summary>
        /// <returns>A list of all greenhouses with eagerly loaded child collections.</returns>
        public async Task<List<Greenhouse>> GetAllAsync()
        {
            try
            {
                return await _context.Greenhouses
                     .Include(G => G.Plants)
                     .Include(G => G.Farmers)
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving greenhouses: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a specific greenhouse by ID with deep navigation properties loaded.
        /// </summary>
        /// <remarks>
        /// This method uses deep Eager Loading to fetch:
        /// - Farmers assigned to the greenhouse
        /// - The Manager of the greenhouse
        /// - The Manager's individual plant responsibilities (FarmerPlants)
        /// - The specific Plant data within those responsibilities
        /// </remarks>
        public async Task<Greenhouse?> GetGreenhouseById(Guid id)
        {
            try
            {
                return await _context.Greenhouses
                    .Include(x => x.Farmers)
                    .ThenInclude(x=>x.FarmerPlants)
                    .ThenInclude(fp => fp.Plant)
                    .Include(x => x.Manager)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error happened while retrieving the greenhouse: {ex.Message}");
            }
        }

        /// <summary>
        /// Finds all greenhouses assigned to a specific manager.
        /// </summary>
        public async Task<List<Greenhouse>?> GetGreenhousesByManagerIdAsync(Guid managerId)
        {
            try
            {
                return await _context.Greenhouses
                    .Where(g => g.ManagerId == managerId)
                    .Include(g => g.Plants)
                    .Include(g => g.Farmers)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving greenhouse by manager id: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves greenhouses that currently do not have a manager assigned.
        /// Used for administrative allocation tasks.
        /// </summary>
        public async Task<List<Greenhouse>?> GetGreenhousesWithoutManagerAsync()
        {
            try
            {
                return await _context.Greenhouses
                    .Where(g => g.ManagerId == null)
                    .Include(g => g.Plants)
                    .Include(g => g.Farmers)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving greenhouses without manager: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the attributes of an existing greenhouse record.
        /// </summary>
        public async Task UpdateAsync(Greenhouse greenhouse)
        {
            try
            {
                _context.Greenhouses.Update(greenhouse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while Updating the GreenHouse : {ex.Message}");
            }
        }
    }
}