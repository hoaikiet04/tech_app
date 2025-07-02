using MemoryImage.Data.Repositories;
using MemoryImage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    // Interface mới cho AdminService
    public interface IAdminService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
    }

    // Class triển khai AdminService
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;

        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Lấy tất cả người dùng, kể cả người dùng không hoạt động
            return await _userRepository.GetAllAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            // Giả sử bạn đã cấu hình Cascade Delete trong DbContext
            return await _userRepository.DeletePermanentlyAsync(userId);
        }
    }
}