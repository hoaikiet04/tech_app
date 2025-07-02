// SocialConnect.Data/Repositories/IPostRepository.cs
using MemoryImage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemoryImage.Data.Repositories
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(int id);
        Task<List<Post>> GetPostsByUserIdAsync(int userId, int take = 10, int skip = 0);
        Task<List<Post>> GetFriendsPostsAsync(List<int> friendIds, int take = 10, int skip = 0); // Lấy bài đăng của bạn bè
        Task<Post> AddAsync(Post post);
        Task<Post> UpdateAsync(Post post);
        Task<bool> DeleteAsync(int id);
    }
}