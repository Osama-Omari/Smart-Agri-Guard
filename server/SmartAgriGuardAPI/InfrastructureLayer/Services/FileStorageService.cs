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
    /// <summary>
    /// Service for managing physical file storage on the server.
    /// Handles saving images, validating file extensions, and performing cleanup of deleted assets.
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment webHostEnviroment)
        {
            _env = webHostEnviroment;
        }

        /// <summary>
        /// Permanently deletes a file from the server's storage.
        /// </summary>
        /// <param name="filePath">The relative path to the file stored in the database.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        public async Task DeleteFileAsync(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            // Resolve the physical root path (handles development vs production environments)
            var uploadsRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Convert relative web paths to the server's specific directory separators
            var fullPath = Path.Combine(uploadsRoot, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(fullPath))
            {
                // Run the deletion on a background thread to avoid blocking the main async flow
                await Task.Run(() => File.Delete(fullPath));
            }
        }

        /// <summary>
        /// Validates and saves an uploaded image file to a designated folder.
        /// </summary>
        /// <param name="file">The file data DTO containing the stream and filename.</param>
        /// <param name="subFolder">Optional subfolder (e.g., "greenhouses" or "plants") to organize assets.</param>
        /// <returns>The relative web path to the saved file.</returns>
        /// <exception cref="ArgumentException">Thrown if the file stream is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the file extension is not an allowed image type.</exception>
        public async Task<string> SaveFileAsync(FileDataDTO file, string subFolder = "")
        {
            if (file?.Content == null || file.Content.Length == 0)
                throw new ArgumentException("File stream is empty.", nameof(file));

            // Security: Whitelist allowed image extensions to prevent script execution attacks
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Invalid file type. Only images are allowed.");

            // Construct the path to the wwwroot/uploads directory
            var uploadsRoot = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads");

            // Ensure the base uploads directory exists
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            // Navigate to the specific sub-category folder
            var targetFolder = string.IsNullOrWhiteSpace(subFolder)
                ? uploadsRoot
                : Path.Combine(uploadsRoot, subFolder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // Generate a Unique filename using GUID to prevent overwriting files with the same name
            var uniqueFileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            // Write the stream to the physical disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.Content.CopyToAsync(stream);
            }

            // Return a standardized relative path for storage in the database (e.g., uploads/plants/unique-id.jpg)
            var relativePath = Path.Combine("uploads", subFolder ?? "", uniqueFileName)
                               .Replace("\\", "/");

            return relativePath;
        }
    }
}