using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment webHostEnviroment)
        {
            _env = webHostEnviroment;
        }

        public async Task<string> SaveFileAsync(FileDataDTO file, string subFolder = "")
        {
            if (file?.Content == null || file.Content.Length == 0)
                throw new ArgumentException("File stream is empty.", nameof(file));

            var uploadsRoot = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads");

            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var targetFolder = string.IsNullOrWhiteSpace(subFolder)
                ? uploadsRoot
                : Path.Combine(uploadsRoot, subFolder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.Content.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", subFolder ?? "", uniqueFileName)
                               .Replace("\\", "/");

            return relativePath;
        }
    }
}
