using ApplicationLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IPlantHealthService
    {
        Task GeneratePlantHealth(Guid PlantId,TomatoHealthInput input);

        Task EvaluateAndAlertPlantHealth(Plant plant,SensorData latestData);
    }
}
