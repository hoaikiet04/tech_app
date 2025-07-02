using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    public interface IFileStorageService
    {
        Task<string?> SaveFileAsync(IFormFile file, string subfolder);
        void DeleteFile(string? relativePath);
    }
}