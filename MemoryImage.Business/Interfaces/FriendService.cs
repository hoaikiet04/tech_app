using MemoryImage.Models;
using MemoryImage.Data.Repositories;

namespace MemoryImage.Business.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        
        public FriendService(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
        }
        
        public async Task<List<User>> GetFriendsAsync(int userId)
        {
            return await _friendshipRepository.GetFriendsAsync(userId);
        }
        
        public async Task<List<User>> GetFriendSuggestionsAsync(int userId, int count = 10)
        {
            return await _userRepository.GetFriendSuggestionsAsync(userId, count);
        }
        
        public async Task<List<Friendship>> GetPendingRequestsAsync(int userId)
        {
            return await _friendshipRepository.GetPendingRequestsAsync(userId);
        }
        
        public async Task<bool> SendFriendRequestAsync(int requesterId, int receiverId)
        {
            if (requesterId == receiverId)
                return false;
                
            var result = await _friendshipRepository.SendFriendRequestAsync(requesterId, receiverId);
            return result != null;
        }
        
        public async Task<bool> AcceptFriendRequestAsync(int friendshipId)
        {
            return await _friendshipRepository.AcceptFriendRequestAsync(friendshipId);
        }
        
        public async Task<bool> DeclineFriendRequestAsync(int friendshipId)
        {
            return await _friendshipRepository.DeclineFriendRequestAsync(friendshipId);
        }
        
        public async Task<bool> RemoveFriendAsync(int userId1, int userId2)
        {
            return await _friendshipRepository.RemoveFriendAsync(userId1, userId2);
        }
        
        public async Task<Dictionary<int, FriendshipStatus?>> GetFriendshipStatusesAsync(int currentUserId, IEnumerable<int> userIds)
        {
            return await _friendshipRepository.GetFriendshipStatusesAsync(currentUserId, userIds);
        }
        
        public async Task<int> GetPendingRequestCountAsync(int userId)
        {
            return await _friendshipRepository.GetPendingRequestCountAsync(userId);
        }
        
        public async Task<FriendshipStatus?> GetFriendshipStatusAsync(int userId1, int userId2)
        {
            return await _friendshipRepository.GetFriendshipStatusAsync(userId1, userId2);
        }
    }
}