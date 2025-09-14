using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ISensorDataService
    {
        Task DeleteAllByPlantIdAsync(Guid plantId);

        Task AddSensorData(Guid plantId,SensorDataRegisterDTO dto);

        Task<SensorDataDTO?> GetLatestSensorData(Guid plantId);

        Task<SensorTrendResponseDTO> GetSensorTrendsAsync(SensorTrendRequestDTO dto);

        
    }
}
