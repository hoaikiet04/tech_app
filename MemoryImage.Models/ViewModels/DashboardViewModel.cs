using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using MemoryImage.Models; // Thêm using này

namespace MemoryImage.Models.ViewModels
{
    public class DashboardViewModel
    {
        public User CurrentUser { get; set; }
        public List<User> Friends { get; set; }
        public List<User> FriendSuggestions { get; set; }
        public List<Friendship> PendingRequests { get; set; }
        public int FriendRequestCount { get; set; }
        public List<Post> NewsFeedPosts { get; set; } 
        public CreatePostViewModel CreatePost { get; set; } = new CreatePostViewModel();
        public Dictionary<int, int> LikeCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, bool> UserLikeStatus { get; set; } = new Dictionary<int, bool>();

        // Thêm thuộc tính để lưu trạng thái bạn bè với các tác giả bài viết
        public Dictionary<int, FriendshipStatus?> FriendshipStatuses { get; set; } = new Dictionary<int, FriendshipStatus?>();
        public Dictionary<int, List<Comment>> Comments { get; set; } = new Dictionary<int, List<Comment>>();
        
    }

    public class CreatePostViewModel
    {
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string? Content { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}