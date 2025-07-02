using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemoryImage.Business.Services;
using System.Threading.Tasks;
using MemoryImage.Models.ViewModels; // Thêm using mới

namespace MemoryImage.Web.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IPostService _postService; // Inject IPostService

        public AdminController(IAdminService adminService, IPostService postService)
        {
            _adminService = adminService;
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllUsersAsync();
            // Lấy 20 bài viết gần đây nhất để quản lý
            var posts = await _postService.GetUserPostsAsync(0, 20, 0); // Lấy bài viết của mọi user

            var viewModel = new AdminDashboardViewModel
            {
                Users = users,
                RecentPosts = posts
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _adminService.DeleteUserAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user. Cannot delete an Admin account.";
            }
            return RedirectToAction("Index");
        }

        // ACTION MỚI ĐỂ XÓA BÀI VIẾT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var success = await _postService.DeletePostAsAdminAsync(postId);
            if(success)
            {
                TempData["SuccessMessage"] = $"Post with ID {postId} deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete post. Cannot delete a post by an Admin.";
            }
            return RedirectToAction("Index");
        }
    }
}