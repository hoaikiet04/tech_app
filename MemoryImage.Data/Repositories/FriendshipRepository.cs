using Microsoft.EntityFrameworkCore;
using MemoryImage.Models;

namespace MemoryImage.Data.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext _context;
        
        public FriendshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<User>> GetFriendsAsync(int userId)
        {
            var friendships = await _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && 
                           f.Status == FriendshipStatus.Accepted)
                .ToListAsync();
                
            return friendships.Select(f => f.RequesterId == userId ? f.Receiver : f.Requester).ToList();
        }
        
        public async Task<List<Friendship>> GetPendingRequestsAsync(int userId)
        {
            return await _context.Friendships
                .Include(f => f.Requester)
                .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<Friendship> SendFriendRequestAsync(int requesterId, int receiverId)
        {
            // Check if friendship already exists
            var existing = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == requesterId && f.ReceiverId == receiverId) ||
                    (f.RequesterId == receiverId && f.ReceiverId == requesterId));
                    
            if (existing != null)
                return null; // Friendship already exists
                
            var friendship = new Friendship
            {
                RequesterId = requesterId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
            return friendship;
        }
        
        public async Task<bool> AcceptFriendRequestAsync(int friendshipId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship != null && friendship.Status == FriendshipStatus.Pending)
            {
                friendship.Status = FriendshipStatus.Accepted;
                friendship.ResponsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        public async Task<bool> DeclineFriendRequestAsync(int friendshipId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship != null && friendship.Status == FriendshipStatus.Pending)
            {
                friendship.Status = FriendshipStatus.Declined;
                friendship.ResponsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        public async Task<bool> RemoveFriendAsync(int userId1, int userId2)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                    (f.RequesterId == userId2 && f.ReceiverId == userId1));
                    
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        public async Task<Dictionary<int, FriendshipStatus?>> GetFriendshipStatusesAsync(int currentUserId, IEnumerable<int> userIds)
        {
            if (userIds == null || !userIds.Any())
                return new Dictionary<int, FriendshipStatus?>();

            var friendships = await _context.Friendships
                .Where(f => 
                    (f.RequesterId == currentUserId && userIds.Contains(f.ReceiverId)) ||
                    (userIds.Contains(f.RequesterId) && f.ReceiverId == currentUserId))
                .ToListAsync();

            return userIds.ToDictionary(
                id => id,
                id => friendships.FirstOrDefault(f => 
                    (f.RequesterId == currentUserId && f.ReceiverId == id) || 
                    (f.RequesterId == id && f.ReceiverId == currentUserId))?.Status
            );
        }
        public async Task<FriendshipStatus?> GetFriendshipStatusAsync(int userId1, int userId2)
        {
            var friendship = await _context.Friendships
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == userId1 && f.ReceiverId == userId2) ||
                    (f.RequesterId == userId2 && f.ReceiverId == userId1));
            
            return friendship?.Status;
        }
        public async Task<int> GetPendingRequestCountAsync(int userId)
        {
            return await _context.Friendships
                .CountAsync(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending);
        }
    }
}