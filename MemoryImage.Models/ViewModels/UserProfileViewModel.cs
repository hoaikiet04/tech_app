using System.Collections.Generic;
using MemoryImage.Models;

namespace MemoryImage.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public bool IsCurrentUser { get; set; }
        public List<Post> Posts { get; set; }
        public FriendshipStatus? FriendshipStatus { get; set; }

        // Thêm các thuộc tính để hiển thị thông tin like trên trang cá nhân
        public Dictionary<int, int> LikeCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, bool> UserLikeStatus { get; set; } = new Dictionary<int, bool>();
        public Dictionary<int, List<Comment>> Comments { get; set; } = new Dictionary<int, List<Comment>>();
        public User LoggedInUser { get; set; } 
    }
}