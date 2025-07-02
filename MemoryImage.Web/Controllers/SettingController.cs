using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemoryImage.Business.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MemoryImage.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;

        public SettingsController(IUserService userService)
        {
            _userService = userService;
        }

        // Action GET để hiển thị trang cài đặt
        public IActionResult Index()
        {
            return View();
        }

        // Action POST để thực hiện xóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) return Unauthorized();
            var userId = int.Parse(userIdString);

            var success = await _userService.DeleteAccountAsync(userId);

            if (success)
            {
                // Đăng xuất người dùng sau khi xóa tài khoản
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["SuccessMessage"] = "Tài khoản của bạn đã được xóa vĩnh viễn.";
                return RedirectToAction("Login", "Account");
            }

            TempData["ErrorMessage"] = "Đã có lỗi xảy ra khi xóa tài khoản của bạn.";
            return RedirectToAction("Index");
        }
    }
}