using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookNow.Web.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fs);
            }

          
          
            return $"/uploads/{folder}/{safeFileName}";
        }

        public Task DeleteFileAsync(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl))
                return Task.CompletedTask;

            var trimmed = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var absolute = Path.Combine(_env.WebRootPath, trimmed);

            try
            {
                if (File.Exists(absolute))
                    File.Delete(absolute);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {Path}", absolute);
            }

            return Task.CompletedTask;
        }
    }
}
