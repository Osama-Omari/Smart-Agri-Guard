using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ISensorDataArchiveService
    {
        Task DeleteAllByPlantIdAsync(Guid PlantId);
    }
}
