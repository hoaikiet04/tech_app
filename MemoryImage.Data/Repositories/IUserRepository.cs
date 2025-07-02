using MemoryImage.Models;

namespace MemoryImage.Data.Repositories
{
    public partial interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(string email);
        Task<List<User>> GetFriendSuggestionsAsync(int userId, int count = 10);
        Task<List<User>> SearchUsersAsync(string searchTerm, int currentUserId);
        
        // Thêm phương thức xóa vĩnh viễn
        Task<bool> DeletePermanentlyAsync(int id); 
    }
}