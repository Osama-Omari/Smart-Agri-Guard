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

        public async Task DeleteFileAsync(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            
            var uploadsRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"); 
            var fullPath = Path.Combine(uploadsRoot, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if(File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }

        public async Task<string> SaveFileAsync(FileDataDTO file, string subFolder = "")
        {
            if (file?.Content == null || file.Content.Length == 0)
                throw new ArgumentException("File stream is empty.", nameof(file));

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Invalid file type. Only images are allowed.");


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
