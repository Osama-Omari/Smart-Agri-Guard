using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(FileDataDTO file, string subFolder = "");
    }
}
