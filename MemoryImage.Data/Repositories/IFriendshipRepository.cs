using MemoryImage.Models;

namespace MemoryImage.Data.Repositories
{
    public interface IFriendshipRepository
    {
        Task<List<User>> GetFriendsAsync(int userId);
        Task<List<Friendship>> GetPendingRequestsAsync(int userId);
        Task<Friendship> SendFriendRequestAsync(int requesterId, int receiverId);
        Task<bool> AcceptFriendRequestAsync(int friendshipId);
        Task<bool> DeclineFriendRequestAsync(int friendshipId);
        Task<bool> RemoveFriendAsync(int userId1, int userId2);
        Task<Dictionary<int, FriendshipStatus?>> GetFriendshipStatusesAsync(int currentUserId, IEnumerable<int> userIds);
        Task<int> GetPendingRequestCountAsync(int userId);
        Task<FriendshipStatus?> GetFriendshipStatusAsync(int userId1, int userId2);

    }
}