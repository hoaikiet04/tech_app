using Microsoft.EntityFrameworkCore;
using MemoryImage.Models;
using System.Linq;

namespace MemoryImage.Data.Repositories
{
    public partial class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }
        
        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await UpdateAsync(user);
                return true;
            }
            return false;
        }
        
        public async Task<bool> ExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        
        public async Task<List<User>> GetFriendSuggestionsAsync(int userId, int count = 10)
        {
            var friendIds = await _context.Friendships
                .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) &&
                            f.Status == FriendshipStatus.Accepted)
                .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
                .ToListAsync();

            var pendingIds = await _context.Friendships
                .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) &&
                            f.Status == FriendshipStatus.Pending)
                .Select(f => f.RequesterId == userId ? f.ReceiverId : f.RequesterId)
                .ToListAsync();

            var excludeIds = friendIds.Concat(pendingIds).Append(userId).ToList();

            return await _context.Users
                .Where(u => u.IsActive && !excludeIds.Contains(u.Id))
                .Take(count)
                .ToListAsync();
        }
        
        public async Task<List<User>> SearchUsersAsync(string searchTerm, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<User>();
            }

            var normalizedSearchTerm = searchTerm.ToLower().Trim();
            
            return await _context.Users
                .Where(u => u.IsActive && u.Id != currentUserId &&
                            (u.FirstName.ToLower().Contains(normalizedSearchTerm) ||
                             u.LastName.ToLower().Contains(normalizedSearchTerm) ||
                             (u.FirstName + " " + u.LastName).ToLower().Contains(normalizedSearchTerm)))
                .Take(20)
                .ToListAsync();
        }

        // Implement phương thức xóa vĩnh viễn
        public async Task<bool> DeletePermanentlyAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // === BẮT ĐẦU VÙNG DỌN DẸP TOÀN DIỆN ===

                // 1. Dọn dẹp Friendships (đã làm ở bước trước)
                var friendshipsAsReceiver = await _context.Friendships
                    .Where(f => f.ReceiverId == id)
                    .ToListAsync();
                if (friendshipsAsReceiver.Any())
                {
                    _context.Friendships.RemoveRange(friendshipsAsReceiver);
                }

                // 2. DỌN DẸP LIKES (MỚI)
                var userLikes = await _context.Likes
                    .Where(l => l.UserId == id)
                    .ToListAsync();
                if (userLikes.Any())
                {
                    _context.Likes.RemoveRange(userLikes);
                }

                // 3. DỌN DẸP COMMENTS (MỚI)
                var userComments = await _context.Comments
                    .Where(c => c.UserId == id)
                    .ToListAsync();
                if (userComments.Any())
                {
                    _context.Comments.RemoveRange(userComments);
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
