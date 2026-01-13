using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IFarmerPlantService
    {

        Task<List<PlantWithMetricsDTO>> GetAssignedPlantsForFarmer(Guid farmerId , string userTimeZoneId);

        Task UnAssignFarmerAsync( Guid plantId , Guid farmerId);

        Task AssignFarmers(Guid plantId, AssignFarmerDTO farmers);
    }
}
