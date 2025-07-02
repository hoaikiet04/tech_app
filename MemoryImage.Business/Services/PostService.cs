using MemoryImage.Data;
using MemoryImage.Data.Repositories;
using MemoryImage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MemoryImage.Business.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IFileStorageService _fileStorageService;
        private readonly ApplicationDbContext _context; 

        public PostService(IPostRepository postRepository, IFileStorageService fileStorageService, ApplicationDbContext context)
        {
            _postRepository = postRepository;
            _fileStorageService = fileStorageService;
            _context = context;
        }
        public async Task<bool> DeletePostAsAdminAsync(int postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return false;

            // Không kiểm tra quyền sở hữu, chỉ kiểm tra xem người bị xóa bài có phải admin không
            if (post.User != null && post.User.IsAdmin)
            {
                // Ngăn chặn admin xóa bài của admin khác
                return false; 
            }

            DeleteImage(post.ImageUrl); // Xóa ảnh liên quan
            return await _postRepository.DeleteAsync(postId); // Gọi repository để xóa
        }
        public async Task<Dictionary<int, List<Comment>>> GetCommentsForPostsAsync(IEnumerable<int> postIds)
        {
            if (postIds == null || !postIds.Any())
            {
                return new Dictionary<int, List<Comment>>();
            }

            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => postIds.Contains(c.PostId))
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            // Gom nhóm các bình luận theo PostId
            return comments
                .GroupBy(c => c.PostId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        public async Task<List<Post>> GetFriendsPostsAsync(List<int> friendIds, int take = 10, int skip = 0)
        {
            return await _postRepository.GetFriendsPostsAsync(friendIds, take, skip);
        }

        public async Task<Comment> AddCommentAsync(int postId, int userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Comment content cannot be empty.");
            var comment = new Comment { PostId = postId, UserId = userId, Content = content, CreatedAt = DateTime.UtcNow };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            // Lấy lại comment cùng với thông tin User để trả về cho client
            return await _context.Comments.Include(c => c.User).FirstAsync(c => c.Id == comment.Id);
        }

        public async Task<List<Comment>> GetCommentsAsync(int postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        #region Các phương thức khác
        private async Task<string?> SaveImageAsync(IFormFile? imageFile, string subfolder)
        {
            if (imageFile == null || imageFile.Length == 0) return null;
            string uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", "images", subfolder);
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create)) { await imageFile.CopyToAsync(fileStream); }
            return $"/images/{subfolder}/{uniqueFileName}";
        }

        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;
            string fullPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot", imageUrl.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<Post> CreatePostAsync(int userId, string? content, IFormFile? imageFile)
        {
            var imageUrl = await _fileStorageService.SaveFileAsync(imageFile, AppConstants.PostsFolderName);

            if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Post must have either content or an image.");
            }

            var post = new Post { UserId = userId, Content = content, ImageUrl = imageUrl, CreatedAt = DateTime.UtcNow };
            return await _postRepository.AddAsync(post);
        }

        public async Task<Post?> GetPostByIdAsync(int postId) => await _postRepository.GetByIdAsync(postId);

        public async Task<List<Post>> GetUserPostsAsync(int userId, int take = 10, int skip = 0) => await _postRepository.GetPostsByUserIdAsync(userId, take, skip);
        
        public async Task<bool> UpdatePostAsync(int postId, int currentUserId, string? content, IFormFile? newImageFile, bool removeExistingImage) 
        { 
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != currentUserId) return false;

            if (removeExistingImage && !string.IsNullOrWhiteSpace(post.ImageUrl))
            {
                DeleteImage(post.ImageUrl);
                post.ImageUrl = null;
            }

            if(newImageFile != null)
            {
                 if (!string.IsNullOrWhiteSpace(post.ImageUrl))
                {
                    DeleteImage(post.ImageUrl);
                }
                post.ImageUrl = await SaveImageAsync(newImageFile, "posts");
            }
            
            post.Content = content;
            post.UpdatedAt = DateTime.UtcNow;
            
            await _postRepository.UpdateAsync(post);
            return true;
        }

        public async Task<bool> DeletePostAsync(int postId, int currentUserId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != currentUserId) return false;

            _fileStorageService.DeleteFile(post.ImageUrl);
            return await _postRepository.DeleteAsync(postId);
        }


        public async Task<(bool success, bool isLiked)> LikeOrUnlikePostAsync(int postId, int userId)
        {
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                await _context.SaveChangesAsync();
                return (true, false);
            }
            else
            {
                await _context.Likes.AddAsync(new Like { PostId = postId, UserId = userId });
                await _context.SaveChangesAsync();
                return (true, true);
            }
        }

        public async Task<Dictionary<int, int>> GetLikeCountsAsync(IEnumerable<int> postIds)
        {
            if (!postIds.Any()) return new Dictionary<int, int>();
            return await _context.Likes
                .Where(l => postIds.Contains(l.PostId))
                .GroupBy(l => l.PostId)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PostId, x => x.Count);
        }

        public async Task<Dictionary<int, bool>> GetUserLikeStatusAsync(IEnumerable<int> postIds, int userId)
        {
            if (!postIds.Any()) return new Dictionary<int, bool>();
            var userLikes = await _context.Likes
                .Where(l => l.UserId == userId && postIds.Contains(l.PostId))
                .Select(l => l.PostId)
                .ToHashSetAsync();
            return postIds.ToDictionary(id => id, id => userLikes.Contains(id));
        }
        #endregion
    }
    
}
