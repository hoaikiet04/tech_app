using MemoryImage.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemoryImage.Business.Services
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(int userId, string? content, IFormFile? imageFile);
        Task<Post?> GetPostByIdAsync(int postId);
        Task<List<Post>> GetUserPostsAsync(int userId, int take = 10, int skip = 0);
        Task<List<Post>> GetFriendsPostsAsync(List<int> friendIds, int take = 10, int skip = 0); 
        Task<bool> UpdatePostAsync(int postId, int currentUserId, string? content, IFormFile? newImageFile, bool removeExistingImage);
        Task<bool> DeletePostAsync(int postId, int currentUserId);
        Task<(bool success, bool isLiked)> LikeOrUnlikePostAsync(int postId, int userId);
        
        // Thêm các phương thức còn thiếu cho chức năng bình luận
        Task<Comment> AddCommentAsync(int postId, int userId, string content);
        Task<List<Comment>> GetCommentsAsync(int postId);
        
        Task<Dictionary<int, int>> GetLikeCountsAsync(IEnumerable<int> postIds);
        Task<Dictionary<int, bool>> GetUserLikeStatusAsync(IEnumerable<int> postIds, int userId);
        Task<Dictionary<int, List<Comment>>> GetCommentsForPostsAsync(IEnumerable<int> postIds);
        Task<bool> DeletePostAsAdminAsync(int postId);
    }
}