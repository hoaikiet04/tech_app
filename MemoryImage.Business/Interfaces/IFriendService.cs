using MemoryImage.Models;

namespace MemoryImage.Business.Services
{
    public interface IFriendService
    {
        Task<List<User>> GetFriendsAsync(int userId);
        Task<List<User>> GetFriendSuggestionsAsync(int userId, int count = 10);
        Task<List<Friendship>> GetPendingRequestsAsync(int userId);
        Task<bool> SendFriendRequestAsync(int requesterId, int receiverId);
        Task<bool> AcceptFriendRequestAsync(int friendshipId);
        Task<bool> DeclineFriendRequestAsync(int friendshipId);
        Task<bool> RemoveFriendAsync(int userId1, int userId2);
        Task<Dictionary<int, FriendshipStatus?>> GetFriendshipStatusesAsync(int currentUserId, IEnumerable<int> userIds);
        Task<int> GetPendingRequestCountAsync(int userId);
        Task<FriendshipStatus?> GetFriendshipStatusAsync(int userId1, int userId2);
    }
}