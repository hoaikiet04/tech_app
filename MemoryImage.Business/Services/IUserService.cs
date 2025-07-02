using MemoryImage.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateProfilePictureAsync(int userId, IFormFile imageFile);
        Task<bool> RemoveProfilePictureAsync(int userId);
        Task<bool> UpdateUserBioAsync(int userId, string bio); 
        
        // Thêm phương thức xóa tài khoản
        Task<bool> DeleteAccountAsync(int userId); 
    }
}