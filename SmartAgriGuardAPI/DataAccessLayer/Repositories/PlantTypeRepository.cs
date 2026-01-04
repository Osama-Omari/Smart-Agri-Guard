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
    /// Repository responsible for managing the classification of plants (Plant Types).
    /// Handles the lookup and persistence of species categories within the system.
    /// </summary>
    public class PlantTypeRepository : IPlantTypeRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public PlantTypeRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a new plant category to the database.
        /// </summary>
        /// <param name="plantType">The PlantType entity to add.</param>
        public async Task AddAsync(PlantType plantType)
        {
            try
            {
                await _context.PlantTypes.AddAsync(plantType);
                await _context.SaveChangesAsync();
            }
            catch { throw new Exception("Error while adding a new plant type "); }
        }

        /// <summary>
        /// Removes a plant category from the database.
        /// </summary>
        /// <param name="id">The GUID of the plant type.</param>
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var plantType = await _context.PlantTypes.FindAsync(id);
                if (plantType != null)
                {
                    _context.PlantTypes.Remove(plantType);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw new Exception("Error while deleting the plant type");
            }
        }

        /// <summary>
        /// Retrieves all plant classifications including the list of plants associated with each type.
        /// </summary>
        public async Task<List<PlantType>> GetAllAsync()
        {
            try
            {
                return await _context.PlantTypes
                    .Include(pt => pt.Plants)
                    .ToListAsync();
            }
            catch
            {
                throw new Exception("Error while getting all plant types");
            }
        }

        /// <summary>
        /// Retrieves a specific plant category by its unique identifier.
        /// Includes eager loading of the associated Plants collection.
        /// </summary>
        public async Task<PlantType?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.PlantTypes
                    .Include(pt => pt.Plants)
                    .FirstOrDefaultAsync(pt => pt.Id == id);
            }
            catch
            {
                throw new Exception("Error while getting plant type");
            }
        }

        /// <summary>
        /// Checks if a plant category name already exists in the system.
        /// Useful for preventing duplicate species entries.
        /// </summary>
        /// <param name="name">The name to check (e.g., "Lavender").</param>
        /// <returns>True if the name exists, otherwise false.</returns>
        public async Task<bool> IsNameExists(string name)
        {
            try
            {
                return await _context.PlantTypes.AnyAsync(pt => pt.Name == name);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while checking for existing plantType name: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the details (Name or Description) of an existing plant category.
        /// </summary>
        public async Task UpdateAsync(PlantType plantType)
        {
            try
            {
                _context.PlantTypes.Update(plantType);
                await _context.SaveChangesAsync();
            }
            catch { throw new Exception("Error while updating the plantType"); }
        }
    }
}