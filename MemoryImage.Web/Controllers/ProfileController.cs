using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MemoryImage.Business.Services;
using MemoryImage.Models.ViewModels;
using MemoryImage.Models;

namespace MemoryImage.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;

        public ProfileController(IUserService userService, IPostService postService, IFriendService friendService)
        {
            _userService = userService;
            _postService = postService;
            _friendService = friendService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserIdString == null) return RedirectToAction("Login", "Account");
            int currentUserId = int.Parse(currentUserIdString);

            // LẤY THÔNG TIN NGƯỜI DÙNG ĐANG ĐĂNG NHẬP
            var loggedInUser = await _userService.GetUserByIdAsync(currentUserId);
            if (loggedInUser == null) return Unauthorized(); // Thêm kiểm tra an toàn

            int userIdToDisplay = id ?? currentUserId;
            var user = (userIdToDisplay == currentUserId) ? loggedInUser : await _userService.GetUserByIdAsync(userIdToDisplay);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            // ... (phần code còn lại của bạn giữ nguyên)
            FriendshipStatus? friendshipStatus = null;
            if (userIdToDisplay != currentUserId)
            {
                friendshipStatus = await _friendService.GetFriendshipStatusAsync(currentUserId, userIdToDisplay);
            }
    
            var userPosts = await _postService.GetUserPostsAsync(userIdToDisplay);
            var postIds = userPosts.Select(p => p.Id);
            var comments = await _postService.GetCommentsForPostsAsync(postIds);

            var profileViewModel = new UserProfileViewModel
            {
                User = user,
                IsCurrentUser = (userIdToDisplay == currentUserId),
                Posts = userPosts,
                FriendshipStatus = friendshipStatus,
                LikeCounts = await _postService.GetLikeCountsAsync(postIds),
                UserLikeStatus = await _postService.GetUserLikeStatusAsync(postIds, currentUserId),
                Comments = comments,
                LoggedInUser = loggedInUser // GÁN VÀO VIEWMODEL
            };

            return View(profileViewModel);
        }
        
        // ACTION MỚI: Xử lý việc cập nhật hồ sơ từ modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            int userId = int.Parse(userIdString);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to update profile. Please check the errors.";
                return RedirectToAction("Index");
            }

            // Cập nhật Bio
            await _userService.UpdateUserBioAsync(userId, model.Bio ?? "");
            
            // Cập nhật ảnh đại diện nếu có file mới
            if (model.ImageFile != null)
            {
                await _userService.UpdateProfilePictureAsync(userId, model.ImageFile);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
        
        // ACTION MỚI: Xử lý xóa ảnh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePicture()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            int userId = int.Parse(userIdString);
            
            await _userService.RemoveProfilePictureAsync(userId);
            
            TempData["SuccessMessage"] = "Profile picture removed.";
            return RedirectToAction("Index");
        }
    }
}