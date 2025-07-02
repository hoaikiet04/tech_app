using Microsoft.EntityFrameworkCore;
using MemoryImage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoryImage.Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        }

        // Trong file PostRepository.cs
        public async Task<List<Post>> GetPostsByUserIdAsync(int userId, int take = 10, int skip = 0)
        {
            var query = _context.Posts.Include(p => p.User).AsQueryable();

            if (userId != 0)
            {
                query = query.Where(p => p.UserId == userId);
            }
    
            return await query.OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        // Hoàn nguyên logic gốc: chỉ lấy bài đăng của bạn bè
        public async Task<List<Post>> GetFriendsPostsAsync(List<int> friendIds, int take = 10, int skip = 0)
        {
            return await _context.Posts
                                 .Include(p => p.User)
                                 .Where(p => friendIds.Contains(p.UserId))
                                 .OrderByDescending(p => p.CreatedAt)
                                 .Skip(skip)
                                 .Take(take)
                                 .ToListAsync();
        }

        public async Task<Post> AddAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
