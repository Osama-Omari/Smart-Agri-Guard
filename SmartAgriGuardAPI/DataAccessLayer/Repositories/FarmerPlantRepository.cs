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
    /// Repository managing the relationship between Farmers and Plants.
    /// Handles the persistence of assignments where specific farmers are linked to specific plants.
    /// </summary>
    public class FarmerPlantRepository : IFarmerPlantRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public FarmerPlantRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a single farmer-to-plant assignment.
        /// </summary>
        /// <param name="obj">The FarmerPlant entity containing the FarmerId and PlantId.</param>
        /// <exception cref="Exception">Thrown if the database update fails.</exception>
        public async Task AddAsync(FarmerPlant obj)
        {
            try
            {
                await _context.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Logs the specific context of the error while preserving the message
                throw new Exception($"Error happened while adding a farmer plant assignment: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs a bulk insertion of multiple farmer-to-plant assignments.
        /// </summary>
        /// <remarks>
        /// This is more efficient than individual inserts for large batches of assignments,
        /// as it reduces the number of round-trips to the database.
        /// </remarks>
        /// <param name="assignments">A list of FarmerPlant entities to be persisted.</param>
        /// <exception cref="Exception">Thrown if the bulk operation fails.</exception>
        public async Task AddAsync(List<FarmerPlant> assignments)
        {
            try
            {
                await _context.FarmerPlants.AddRangeAsync(assignments);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error happened while bulk adding farmer plant assignments: {ex.Message}");
            }
        }
    }
}