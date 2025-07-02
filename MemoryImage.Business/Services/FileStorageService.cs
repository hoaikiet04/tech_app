using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string _storagePath;

        public FileStorageService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _storagePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "images");
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<string?> SaveFileAsync(IFormFile file, string subfolder)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(_storagePath, subfolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{subfolder}/{uniqueFileName}";
        }
    }
}