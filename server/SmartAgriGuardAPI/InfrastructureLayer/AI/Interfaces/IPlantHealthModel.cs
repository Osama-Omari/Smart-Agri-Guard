using ApplicationLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.AI.Interfaces
{
    public interface IPlantHealthModel
    {
        Task<PlantHealthStatus> PredictAsync(TomatoHealthInput input);
    }
}
