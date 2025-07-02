using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MemoryImage.Models.ViewModels;
using MemoryImage.Business.Services;
using System.Security.Claims;
using MemoryImage.Models;
using System.Linq;
using System.Collections.Generic;
using MemoryImage.Data.Repositories;
using MemoryImage.Models;

namespace MemoryImage.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendService _friendService;
        private readonly IPostService _postService;

        public HomeController(IUserRepository userRepository, IFriendService friendService, IPostService postService)
        {
            _userRepository = userRepository;
            _friendService = friendService;
            _postService = postService;
        }

        // Khôi phục Action Index về logic ổn định
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return RedirectToAction("Login", "Account");
            var userId = int.Parse(userIdString);

            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found.");

            // THAY ĐỔI LOGIC: Lấy 20 bài viết mới nhất từ tất cả người dùng
            var newsFeedPosts = await _postService.GetUserPostsAsync(0, 20, 0); 
            var postIds = newsFeedPosts.Select(p => p.Id).ToList();
            
            var authorIds = newsFeedPosts.Select(p => p.UserId).Distinct().ToList();
            var friendshipStatuses = await _friendService.GetFriendshipStatusesAsync(userId, authorIds);

            var viewModel = new DashboardViewModel
            {
                CurrentUser = currentUser,
                Friends = await _friendService.GetFriendsAsync(userId),
                FriendSuggestions = await _friendService.GetFriendSuggestionsAsync(userId, 5),
                PendingRequests = await _friendService.GetPendingRequestsAsync(userId),
                FriendRequestCount = await _friendService.GetPendingRequestCountAsync(userId),
                NewsFeedPosts = newsFeedPosts,
                CreatePost = new CreatePostViewModel(),
                LikeCounts = await _postService.GetLikeCountsAsync(postIds),
                UserLikeStatus = await _postService.GetUserLikeStatusAsync(postIds, userId),
                Comments = await _postService.GetCommentsForPostsAsync(postIds),
                FriendshipStatuses = friendshipStatuses
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind(Prefix = "CreatePost")] CreatePostViewModel model)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return RedirectToAction("Login", "Account");
            var userId = int.Parse(userIdString);

            if (model.ImageFile != null && model.ImageFile.Length > 5 * 1024 * 1024) 
            {
                 ModelState.AddModelError("CreatePost.ImageFile", "Image file is too large. Please upload an image smaller than 5 MB.");
            }

            if (string.IsNullOrWhiteSpace(model.Content) && model.ImageFile == null)
            {
                ModelState.AddModelError("CreatePost", "A post must have either text content or an image.");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    await _postService.CreatePostAsync(userId, model.Content, model.ImageFile);
                    TempData["SuccessMessage"] = "Post created successfully!";
                    return RedirectToAction("Index");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Load lại đầy đủ view model nếu có lỗi
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null) return NotFound("User not found.");
            
            var friendIds = (await _friendService.GetFriendsAsync(userId)).Select(f => f.Id).ToList();
            friendIds.Add(userId);
            
            var viewModel = new DashboardViewModel
            {
                CurrentUser = currentUser,
                Friends = await _friendService.GetFriendsAsync(userId),
                FriendSuggestions = await _friendService.GetFriendSuggestionsAsync(userId, 5),
                PendingRequests = await _friendService.GetPendingRequestsAsync(userId),
                FriendRequestCount = await _friendService.GetPendingRequestCountAsync(userId),
                NewsFeedPosts = await _postService.GetFriendsPostsAsync(friendIds),
                CreatePost = model
            };
            TempData["ErrorMessage"] = "Failed to create post. Please check the errors.";
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return RedirectToAction("Login", "Account");
            var userId = int.Parse(userIdString);

            var post = await _postService.GetPostByIdAsync(id);

            if (post == null || post.UserId != userId)
            {
                TempData["ErrorMessage"] = "Post not found or you don't have permission to edit it.";
                return RedirectToAction("Index");
            }

            var model = new EditPostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                ExistingImageUrl = post.ImageUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Json(new { success = false, message = "Not authenticated." });
            var userId = int.Parse(userIdString);

            var success = await _postService.DeletePostAsync(id, userId);
            if (success)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Failed to delete post." });
        }
    }
}
